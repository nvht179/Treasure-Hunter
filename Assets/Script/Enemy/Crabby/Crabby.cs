using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Crabby : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Player player;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float visionDistance;
    [SerializeField] private float groundAndWallCheckOriginOffset;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float nearPlayerThreshold;
    [SerializeField] private float attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float chargeTime;
    [SerializeField] private float hitTime;
    [SerializeField] private float deadHitTime;
    [SerializeField] private float rechargeTime;
    [SerializeField] private float maxHealth;

    public event EventHandler OnAttack;
    public event EventHandler OnDestroyed;
    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;

    public enum CrabbyState
    {
        Wandering,
        Pursuing,
        Waiting,
        Charging,
        Attacking,
        Recharging,
        Hit,
        DeadHit,
        Dead,
    }

    private Rigidbody2D rb;
    private int moveDirection = 1; // 1 = right, -1 = left
    private CrabbyState state;
    private Vector3 size;
    private Vector3 groundAndWallCheckPosition;
    private float chargeTimer;
    private float rechargeTimer;
    private float hitTimer;
    private float deadHitTimer;
    private float currentHealth;
    private bool hasAttacked;
    private bool isRecharging;

    private const float EyeHeightToBodyRatio = 0.75f;
    private const float GroundCheckDistance = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        size = GetComponent<CapsuleCollider2D>().bounds.size;
        moveDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        chargeTimer = chargeTime;
        rechargeTimer = rechargeTime;
        state = CrabbyState.Wandering;
        currentHealth = maxHealth;
        isRecharging = false;
    }

    private void Update()
    {
        ManageStates();
        groundAndWallCheckPosition = new Vector3(transform.position.x + moveDirection * groundAndWallCheckOriginOffset,
            transform.position.y, transform.position.z);
        attackOrigin.position = new Vector3(moveDirection * attackOrigin.position.x, attackOrigin.position.y,
            attackOrigin.position.z);
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case CrabbyState.Pursuing:
                PursuePlayer();
                break;
            case CrabbyState.Wandering:
                Wander();
                break;
            case CrabbyState.Waiting:
                Wait();
                break;
            case CrabbyState.Charging:
                Charge();
                break;
            case CrabbyState.Attacking:
                Attack();
                break;
            case CrabbyState.Recharging:
                Recharge();
                break;
            case CrabbyState.Hit:
                Hit();
                break;
            case CrabbyState.DeadHit:
                DeadHit();
                break;
            case CrabbyState.Dead:
                Destroy(gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Charge()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        chargeTimer -= Time.fixedDeltaTime;
    }

    private void Recharge()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        rechargeTimer -= Time.fixedDeltaTime;
    }

    private void Attack()
    {
        var playersInRange = new Collider2D[10];
        _ = Physics2D.OverlapCircleNonAlloc(attackOrigin.position, attackRange, playersInRange, playerLayer);
        foreach (var playerCollider in playersInRange)
        {
            if (playerCollider != null)
            {
                var damageable = playerCollider.GetComponent<IDamageable>();
                damageable?.TakeDamage(attackDamage);
            }
        }

        OnAttack?.Invoke(this, EventArgs.Empty);

        hasAttacked = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth = math.clamp(currentHealth - damage, 0, maxHealth);
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealth,
            MaxHealth = maxHealth
        });

        if (currentHealth == 0)
        {
            state = CrabbyState.DeadHit;
            deadHitTimer = deadHitTime;
            return;
        }

        state = CrabbyState.Hit;
        hitTimer = hitTime;
    }

    // TODO: fix crabby not following attacking pattern when pushing it towards a wall
    private void ManageStates()
    {
        if (state == CrabbyState.Dead) return;
        var playerDetectedRight = CastVisionRay(Vector2.right);
        var playerDetectedLeft = CastVisionRay(Vector2.left);

        if (state != CrabbyState.Recharging && state != CrabbyState.Hit && state != CrabbyState.DeadHit)
        {
            if (playerDetectedLeft || playerDetectedRight)
            {
                if (HasContinuousPathToPlayer())
                {
                    if (IsHittingWall())
                    {
                        state = CrabbyState.Waiting;
                    }
                    else
                    {
                        state = CrabbyState.Pursuing;
                        moveDirection = playerDetectedRight ? 1 : -1;
                    }
                }
                else
                {
                    state = CrabbyState.Wandering;
                }
            }
            else
            {
                state = CrabbyState.Wandering;
            }

            if (IsNearPlayer())
            {
                if (chargeTimer > 0)
                {
                    if (state != CrabbyState.Charging)
                    {
                        state = CrabbyState.Charging;
                        hasAttacked = false;
                    }
                }
                else if (chargeTimer <= 0)
                {
                    if (!hasAttacked)
                    {
                        state = CrabbyState.Attacking;
                    }
                    else
                    {
                        state = CrabbyState.Recharging;
                    }
                }
            }
            else
            {
                chargeTimer = chargeTime;
                state = CrabbyState.Wandering;
            }
        }

        if (state == CrabbyState.Recharging)
        {
            isRecharging = true;
            if (rechargeTimer <= 0)
            {
                state = CrabbyState.Wandering;
                chargeTimer = chargeTime;
                rechargeTimer = rechargeTime;
                hasAttacked = false;
                isRecharging = false;
            }
        }

        if (state == CrabbyState.Hit)
        {
            if (isRecharging)
            {
                state = CrabbyState.Recharging;
            }
            else if (hitTimer <= 0)
            {
                state = CrabbyState.Wandering;
            }
        }
        
        if (state == CrabbyState.DeadHit)
        {
            if (deadHitTimer <= 0)
            {
                state = CrabbyState.Dead;
            }
        }
    }

    private bool CastVisionRay(Vector2 direction)
    {
        var rayOrigin = new Vector2(transform.position.x, GetVisionHeight());

        var wallHit = Physics2D.Raycast(rayOrigin, direction, visionDistance, groundLayer);
        var maxDistance = wallHit.collider != null ? wallHit.distance : visionDistance;

        var playerHit = Physics2D.Raycast(rayOrigin, direction, maxDistance, playerLayer);

        return playerHit.collider != null;
    }

    // check if there is ground from crabby to player but does not check if there is low height blocking wall
    private bool HasContinuousPathToPlayer()
    {
        var startPos = transform.position;
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

    private void PursuePlayer()
    {
        var directionToPlayer = player.GetPosition().x > transform.position.x ? 1 : -1;

        rb.velocity = new Vector2(moveSpeed * directionToPlayer, rb.velocity.y);
    }

    private void Wander()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);

        if (!IsGroundAhead() || IsHittingWall())
        {
            moveDirection *= -1;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Wait()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    private void Hit()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        hitTimer -= Time.fixedDeltaTime;
    }
    
    private void DeadHit()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        deadHitTimer -= Time.fixedDeltaTime;
    }

    private bool IsGroundAhead()
    {
        var hit = Physics2D.Raycast(groundAndWallCheckPosition, Vector2.down, groundAndWallCheckOriginOffset,
            groundLayer);
        return hit.collider != null;
    }

    private bool IsHittingWall()
    {
        var dir = moveDirection == 1 ? Vector2.right : Vector2.left;
        var hit = Physics2D.Raycast(groundAndWallCheckPosition, dir, wallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool IsNearPlayer()
    {
        var playerPos = player.GetPosition();
        var crabbyPos = transform.position;
        // Check if the player is within a certain distance (e.g., 1 unit)
        return Mathf.Abs(playerPos.x - crabbyPos.x) < nearPlayerThreshold &&
               Mathf.Abs(playerPos.y - crabbyPos.y) < nearPlayerThreshold;
    }

    public CrabbyState GetState()
    {
        return state;
    }

    public bool IsGrounded()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, groundAndWallCheckOriginOffset,
            groundLayer);
        return hit.collider != null;
    }

    private float GetVisionHeight()
    {
        return transform.position.y + size.y * EyeHeightToBodyRatio;
    }

    public Vector3 GetMoveDirection()
    {
        if (state == CrabbyState.Pursuing)
        {
            return player.GetPosition().x > transform.position.x ? Vector3.right : Vector3.left;
        }

        return moveDirection == 1 ? Vector3.right : Vector3.left;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    private void OnDrawGizmos()
    {
        var groundAndWallCheckPositionGizmos =
            new Vector3(transform.position.x + moveDirection * groundAndWallCheckOriginOffset, transform.position.y,
                transform.position.z);

        var dir = moveDirection == 1 ? Vector2.right : Vector2.left;
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, Vector2.down * GroundCheckDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(groundAndWallCheckPositionGizmos, Vector2.down * GroundCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(groundAndWallCheckPositionGizmos, dir * wallCheckDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(attackOrigin.position, dir * attackRange);

        var rayOrigin = new Vector2(transform.position.x, transform.position.y + GetVisionHeight());
        Gizmos.color = state == CrabbyState.Pursuing ? Color.red : Color.yellow;
        Gizmos.DrawRay(rayOrigin, Vector2.right * visionDistance);
        Gizmos.DrawRay(rayOrigin, Vector2.left * visionDistance);
    }
}