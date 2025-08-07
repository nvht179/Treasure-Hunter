using System;
using Unity.Mathematics;
using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarStateManager : AbstractEnemy, IDamageable
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float chargeSpeed;
        [SerializeField] private float visionDistance;
        [SerializeField] private float attackDamage;
        [SerializeField] private float chargeTime;
        [SerializeField] private float rechargeTime;
        [SerializeField] private Transform pivot; // used for vision and ground/wall ahead checking
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private float knockbackForce;

        private const float GroundCheckDistance = 0.1f;
        private const float GroundAndWallCheckAheadDistance = 0.7f;
        private const int MaxMoneyOnDead = 5;
        public const float AttackRadius = 0.4f;
        public const float HitRecoverTime = 0.5f; // time to recover from being hit
        public const float DeadShowTime = 0.5f; // time for playing the DeadGround animation and showing enemy corpse

        public event EventHandler OnCollisionEnter;
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
        private int moveDirection; // 1 = right, -1 = left

        public float MoveSpeed => moveSpeed;
        public float ChargeSpeed => chargeSpeed;
        public float AttackDamage => attackDamage;
        public float ChargeTime => chargeTime;
        public float RechargeTime => rechargeTime;
        public float KnockbackForce => knockbackForce;
        public Transform Pivot => pivot;
        public LayerMask PlayerLayer => playerLayer;

        public int MoveDirection
        {
            get => moveDirection;
            set => moveDirection = value < 0 ? -1 : 1;
        }

        private PinkStarBaseState currentState;
        public PinkStarWanderState WanderState;
        public PinkStarWaitState WaitState;
        public PinkStarChargeState ChargeState;
        public PinkStarRechargeState RechargeState;
        public PinkStarAttackState AttackState;
        public PinkStarHitState HitState;
        public PinkStarDeadState DeadState;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Collider2D = GetComponent<Collider2D>();
            MoveDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
            CurrentHealth = MaxHealth;
            WanderState = new PinkStarWanderState(this);
            WaitState = new PinkStarWaitState(this);
            ChargeState = new PinkStarChargeState(this);
            RechargeState = new PinkStarRechargeState(this);
            AttackState = new PinkStarAttackState(this);
            HitState = new PinkStarHitState(this);
            DeadState = new PinkStarDeadState(this);
        }

        private void Start()
        {
            currentState = WanderState;
            currentState.EnterState();
        }

        private void Update()
        {
            currentState.UpdateState();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            currentState.OnCollisionEnter(other);
            OnCollisionEnter?.Invoke(this, EventArgs.Empty);
        }

        public bool CastVisionRay(Vector2 direction)
        {
            var wallHit = Physics2D.Raycast(pivot.position, direction, visionDistance, groundLayer);
            var maxDistance = wallHit.collider != null ? wallHit.distance : visionDistance;

            var playerHit = Physics2D.Raycast(pivot.position, direction, maxDistance, playerLayer);

            return playerHit.collider != null;
        }

        public void SwitchState(PinkStarBaseState nextState)
        {
            currentState = nextState;
            currentState.EnterState();
        }

        public bool IsGroundAhead()
        {
            var groundAheadCheckPosition = new Vector2(
                pivot.position.x + MoveDirection * GroundAndWallCheckAheadDistance,
                pivot.position.y);

            // the groundAheadCheckPosition is not the pivot, so the offset must be compensated
            var groundCheckAheadDistance = GroundCheckDistance + (groundAheadCheckPosition.y - transform.position.y);

            var hit = Physics2D.Raycast(groundAheadCheckPosition, Vector2.down, groundCheckAheadDistance, groundLayer);
            return hit.collider != null;
        }

        public bool IsWallAhead()
        {
            var dir = MoveDirection == 1 ? Vector2.right : Vector2.left;
            var hit = Physics2D.Raycast(pivot.position, dir, GroundAndWallCheckAheadDistance, groundLayer);
            return hit.collider != null;
        }

        public bool IsGrounded()
        {
            var hit = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDistance,
                groundLayer);
            return hit.collider != null;
        }

        public bool IsPlayerDetected()
        {
            var playerDetectedRight = CastVisionRay(Vector2.right);
            var playerDetectedLeft = CastVisionRay(Vector2.left);

            return playerDetectedLeft || playerDetectedRight;
        }

        public Vector2 GetVelocity()
        {
            return Rb.velocity;
        }

        public PinkStarState GetCurrentState()
        {
            return currentState.GetCurrentState();
        }

        // check if there is ground from pink star to player but does not check if there is low height blocking wall
        public bool HasContinuousPathToPlayer()
        {
            var startPos = pivot.position;
            var playerPos = Player.GetPosition();

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

        public override void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            currentState.TakeDamage(offenderInfo);
            OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
            {
                CurrentHealth = CurrentHealth,
                MaxHealth = MaxHealth
            });
        }

        public override void SelfDestroy()
        {
            base.SelfDestroy();
            ResourceSpawner.Instance.SpawnMoney(transform.position, 1, MaxMoneyOnDead);
        }

        private void OnDrawGizmos()
        {
            if (pivot == null) return;

            // Draw the GroundAndWallCheckAhead point
            var groundAndWallCheckAheadPos = new Vector2(
                pivot.position.x + MoveDirection * GroundAndWallCheckAheadDistance,
                pivot.position.y
            );
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(groundAndWallCheckAheadPos, 0.07f);
            
            // Draw the ray for ground check ahead
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                groundAndWallCheckAheadPos,
                groundAndWallCheckAheadPos + Vector2.down * (GroundCheckDistance + (groundAndWallCheckAheadPos.y - transform.position.y))
            );

            // Optionally, draw the ray for wall check
            var dir = MoveDirection == 1 ? Vector2.right : Vector2.left;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pivot.position, pivot.position + (Vector3)(dir * GroundAndWallCheckAheadDistance));
        }
    }
}