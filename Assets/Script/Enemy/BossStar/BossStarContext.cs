using System;
using System.Collections.Generic;
using System.Linq;
using Script.ScriptableObjects;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarContext : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float pursueSpeed;
        [SerializeField] private float fleeSpeed;
        [SerializeField] private float attackDamage;
        [SerializeField] private float knockbackForce;
        [SerializeField] private float attackRadius;
        [SerializeField] private Player player;
        [SerializeField] private Transform pivot; // used for vision and ground/wall ahead checking
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float firstStageActiveTime;
        [SerializeField] private float firstStageRestTime;
        [SerializeField] private float secondStageRestTime;
        [SerializeField] private float thirdStageActiveTime;
        [SerializeField] private float thirdStageRestTime;
        [SerializeField] private int secondStageEnemyLimit;
        [SerializeField] private Transform[] enemySpawnPositions;
        [SerializeField] private Transform[] fleePositions;
        [SerializeField] private Transform centralPosition;
        [SerializeField] private FlyingObjectSO[] bulletTypes;
        [SerializeField] private EnemySO[] enemyTypes;

        private const int FiringPointsNum = 16;
        public const float HitRecoverTime = 0.5f; // time to recover from being hit
        public const float DeadShowTime = 0.5f; // time to for playing the DeadGround animation and showing enemy corpse
        public const float SecondStageMaxHealthRatio = 0.7f;
        public const float ThirdStageMaxHealthRatio = 0.4f;

        /*
         * when the boss received damage equal to FleePercentageMaxHealthThreshold * MaxHealth
         * second stage: attack
         * third stage: flee
         */
        public const float FleePercentageMaxHealthThreshold = 0.05f;

        /*
         * currently there are 3 bullet patterns in total
         * spray: fire from all firing points outwards with low cooldown.
         * barrel: fire from all the firing points in the player direction with moderate cooldown
         * snipe: fire from the central with very high speed and moderate cooldown
         */

        public const float LowCooldown = 0.5f;
        public const float MediumCooldown = 3f;
        public const float HighCooldown = 7f;
        public const int MaxConsecutive = 5; // maximum number of the same consecutive bullet pattern

        public event EventHandler OnDestroyed;
        public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;

        public Rigidbody2D Rb { get; private set; }
        public Collider2D Collider2D { get; private set; }
        public List<Transform> FiringPoints { get; private set; }
        public List<Vector3> SprayFiringDirections { get; private set; }
        public HashSet<AbstractEnemy> AliveEnemies { get; private set; }
        private float currentHealth;
        private int moveDirection; // 1 = right, -1 = left

        private bool isActive;

        public Player Player => player;
        public float PursueSpeed => pursueSpeed;
        public float FleeSpeed => fleeSpeed;
        public float AttackDamage => attackDamage;
        public float KnockbackForce => knockbackForce;
        public float AttackRadius => attackRadius;
        public float FirstStageActiveTime => firstStageActiveTime;
        public float FirstStageRestTime => firstStageRestTime;
        public float SecondStageRestTime => secondStageRestTime;
        public float ThirdStageActiveTime => thirdStageActiveTime;
        public float ThirdStageRestTime => thirdStageRestTime;
        public float SecondStageEnemyLimit => secondStageEnemyLimit;
        public FlyingObjectSO[] BulletTypes => bulletTypes;
        public EnemySO[] EnemyTypes => enemyTypes;
        public Transform[] EnemySpawnPositions => enemySpawnPositions;
        public Transform[] FleePositions => fleePositions;
        public Transform Pivot => pivot;
        public LayerMask PlayerLayer => playerLayer;
        public bool SecondStageInit { get; private set; }

        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }

        public float MaxHealth => maxHealth;

        public int MoveDirection
        {
            get => moveDirection;
            set => moveDirection = value < 0 ? -1 : 1;
        }

        public Transform CentralPosition => centralPosition;

        private BossStarBaseStage currentState;
        public BossStarFirstStageActive FirstStageActive;
        public BossStarFirstStageRest FirstStageRest;
        public BossStarSecondStageActive SecondStageActive;
        public BossStarSecondStageRest SecondStageRest;
        public BossStarThirdStageActive ThirdStageActive;
        public BossStarThirdStageRest ThirdStageRest;
        public BossStarDead Dead;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Collider2D = GetComponent<Collider2D>();
            MoveDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            CurrentHealth = maxHealth;
            FirstStageActive = new BossStarFirstStageActive(this);
            FirstStageRest = new BossStarFirstStageRest(this);
            SecondStageActive = new BossStarSecondStageActive(this);
            SecondStageRest = new BossStarSecondStageRest(this);
            ThirdStageActive = new BossStarThirdStageActive(this);
            ThirdStageRest = new BossStarThirdStageRest(this);
            Dead = new BossStarDead(this);
            SprayFiringDirections = new List<Vector3>();
            AliveEnemies = new HashSet<AbstractEnemy>();
            currentState = FirstStageActive;
        }

        private void Start()
        {
            currentState = FirstStageActive;
            currentState.EnterState();

            // calculate fire points
            FiringPoints = new List<Transform>(16);
            const float distanceFromPivot = 1.5f;
            const float angleStep = 360f / FiringPointsNum;
            for (var i = 0; i < FiringPointsNum; i++)
            {
                var angleDeg = 90f - i * angleStep; // start at North (90°), go clockwise
                var angleRad = angleDeg * Mathf.Deg2Rad;
                var offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f) * distanceFromPivot;
                var point = new GameObject($"FiringPoint_{i}")
                {
                    transform =
                    {
                        position = pivot.position + offset,
                        parent = pivot
                    }
                };
                FiringPoints.Add(point.transform);

                // calculate normalized direction from pivot to firing point
                var direction = (point.transform.position - pivot.position).normalized;
                SprayFiringDirections.Add(direction);
            }
        }

        private void Update()
        {
            currentState.UpdateState();
        }

        public void SwitchState(BossStarBaseStage nextState)
        {
            currentState = nextState;
            currentState.EnterState();
        }

        public Vector2 GetVelocity()
        {
            return Rb.velocity;
        }

        public BossStarBaseStage GetCurrentState()
        {
            return currentState;
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }

        public void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            currentState.TakeDamage(offenderInfo);
            OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
            {
                CurrentHealth = CurrentHealth,
                MaxHealth = maxHealth
            });
        }

        public void InitSecondStage()
        {
            SecondStageInit = true;
        }
        
        public void RegisterEnemy(AbstractEnemy enemy)
        {
            if (AliveEnemies.Add(enemy))
                enemy.OnDestroyed += EnemyOnDestroyed;
        }

        private void EnemyOnDestroyed(object sender, AbstractEnemy.OnDestroyedEventArgs e)
        {
            UnregisterEnemy(e.Enemy);
        }

        public void UnregisterEnemy(AbstractEnemy enemy)
        {
            if (AliveEnemies.Remove(enemy))
                enemy.OnDestroyed -= EnemyOnDestroyed;
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }

        public bool IsActive()
        {
            return isActive;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pivot.position, attackRadius);
        }
    }
}