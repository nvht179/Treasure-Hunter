using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Crabby : MonoBehaviour
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
    [SerializeField] private float rechargeTime;

    public event EventHandler OnAttack;

    private enum CrabbyState
    {
        Wandering,
        Pursuing,
        Waiting,
        Charging,
        Attacking,
        Recharging
    }

    private Rigidbody2D rb;
    private int moveDirection = 1; // 1 = right, -1 = left
    private CrabbyState state;
    private Vector3 size;
    private Vector3 groundAndWallCheckPosition;
    private float chargeTimer;
    private float rechargeTimer;
    private bool hasAttacked;

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
    }

    private void Update()
    {
        CheckForPlayer();
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

        if (rechargeTimer <= 0)
        {
            state = CrabbyState.Wandering;
            chargeTimer = chargeTime;
            rechargeTimer = rechargeTime;
            hasAttacked = false;
        }
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

    private void CheckForPlayer()
    {
        var playerDetectedRight = CastVisionRay(Vector2.right);
        var playerDetectedLeft = CastVisionRay(Vector2.left);

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
            else if (chargeTimer <= 0 && !hasAttacked)
            {
                state = CrabbyState.Attacking;
            }
            else
            {
                state = CrabbyState.Recharging;
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

    public bool IsCharging()
    {
        return state == CrabbyState.Charging;
    }

    public bool IsGrounded()
    {
        var hit = Physics2D.Raycast(transform.position,Vector2.down, groundAndWallCheckOriginOffset,
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