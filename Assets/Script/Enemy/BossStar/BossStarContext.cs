using System;
using Unity.Mathematics;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarContext : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float pursueSpeed;
        [SerializeField] private float fleeSpeed;
        [SerializeField] private float attackDamage;
        [SerializeField] private Player player;
        [SerializeField] private Transform pivot; // used for vision and ground/wall ahead checking
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float firstStageActiveTime;
        [SerializeField] private float firstStageRestTime;
        [SerializeField] private float secondStageRestTime;
        [SerializeField] private float thirdStageActiveTime;
        [SerializeField] private float thirdStageRestTime;

        private const float GroundCheckDistance = 0.1f;
        private const float GroundAndWallCheckAheadDistance = 0.7f;
        public const float HitRecoverTime = 0.5f; // time to recover from being hit
        public const float DeadShowTime = 0.5f; // time to for playing the DeadGround animation and showing enemy corpse

        public event EventHandler OnDestroyed;
        public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;

        public enum PinkStarState
        {
            Wander,
            Wait,
            Charge,
            Attack,
            Recharge,
            Hit,
            DeadHit,
            Dead,
        }

        public Rigidbody2D Rb { get; private set; }
        public Collider2D Collider2D { get; private set; }
        private float currentHealth;
        private int moveDirection; // 1 = right, -1 = left

        public float PursueSpeed => pursueSpeed;
        public float FleeSpeed => fleeSpeed;
        public float AttackDamage => attackDamage;
        public float FirstStageActiveTime => firstStageActiveTime;
        public float FirstStageRestTime => firstStageRestTime;
        public float SecondStageRestTime => secondStageRestTime;
        public float ThirdStageActiveTime => thirdStageActiveTime;
        public float ThirdStageRestTime => thirdStageRestTime;

        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }

        public int MoveDirection
        {
            get => moveDirection;
            set => moveDirection = value < 0 ? -1 : 1;
        }

        private BossStarBaseStage currentState;
        public BossStarFirstStageActive FirstStageActive;
        public BossStarFirstStageRest FirstStageRest;
        public BossStarSecondStageActive SecondStageActive;
        public BossStarSecondStageRest SecondStageRest;
        public BossStarThirdStageActive BossStarThirdStageActive;
        public BossStarThirdStageRest BossStarThirdStageRest;
        public BossStarDead BossStarDead;

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
            BossStarThirdStageActive = new BossStarThirdStageActive(this);
            BossStarThirdStageRest = new BossStarThirdStageRest(this);
            BossStarDead = new BossStarDead(this);
            currentState = FirstStageActive;
        }

        private void Start()
        {
            currentState = FirstStageActive;
            currentState.EnterState();
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

        // check if there is ground from pink star to player but does not check if there is low height blocking wall
        public bool HasContinuousPathToPlayer()
        {
            var startPos = pivot.position;
            var playerPos = player.GetPosition();

            // determine direction to player
            var directionToPlayer = playerPos.x > startPos.x ? 1f : -1f;
            var distanceToPlayer = math.abs(playerPos.x - startPos.x);

            // sample points along the path to check for ground continuity
            const float sampleInterval = 0.5f;
            var sampleCount = Mathf.CeilToInt(distanceToPlayer / sampleInterval); // check every 0.5 units
            sampleCount = Mathf.Max(sampleCount, 1);

            for (var i = 1; i <= sampleCount; i++)
            {
                var sampleDistance = distanceToPlayer / sampleCount * i;
                var checkPos = new Vector2(startPos.x + directionToPlayer * sampleDistance, startPos.y);

                var groundHit = Physics2D.Raycast(checkPos, Vector2.down, GroundCheckDistance, groundLayer);

                if (groundHit.collider == null)
                {
                    return false;
                }
            }

            return true;
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }

        public void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            currentState.TakeDamage(offenderInfo.Damage);
            OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
            {
                CurrentHealth = CurrentHealth,
                MaxHealth = maxHealth
            });
        }
    }
}