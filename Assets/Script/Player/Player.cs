using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Jump System")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float jumpDegrader;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform playerPivot;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Attack")]
    [SerializeField] private int playerDamage;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float maxHealthPoint;
    
    private const float GroundCheckRadius = 0.05f;

    public event EventHandler OnDestroyed;
    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;

    private float gravityScale;
    private float currentHealthPoint;
    private bool isGrounded;
    private bool isRunning;
    private bool isConstantlyAttacking;
    private bool isAttacking;
    private bool isDamaged;
    private bool hasAirAttacked;
    private float cooldownTimer;
    private Vector3 moveDir;
    private Vector2 gravityVector;
    public bool IsFacingRight { get; set; }
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        IsFacingRight = true;
        isConstantlyAttacking = false;
        hasAirAttacked = false;
        currentHealthPoint = maxHealthPoint;
        gravityScale = rb.gravityScale;
    }

    private void Start()
    {
        gameInput.OnJumpAction += PlayerOnJump;
        gameInput.OnAttackAction += PlayerOnAttack;
        gravityVector = new Vector2(0, -Physics2D.gravity.y);
    }

    private void PlayerOnJump(object sender, EventArgs e)
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
    }

    private void PlayerOnAttack(object sender, EventArgs e)
    {
        isConstantlyAttacking = !isConstantlyAttacking;
    }

    private void Update()
    {
        // handle input
        var inputVector = gameInput.GetMovementVectorNormalized();
        moveDir = new Vector3(inputVector.x, inputVector.y, 0);

        // handle actions
        HandleRunning();
        HandleJumping();
        HandleAttacking();

        // update states
        isRunning = moveDir.x != 0f;
        isGrounded = Physics2D.OverlapCircle(playerPivot.position, GroundCheckRadius, groundLayer);
        if (isGrounded)
        {
            hasAirAttacked = false;
        }
    }

    private void HandleAttacking()
    {
        if (cooldownTimer <= 0)
        {
            if (isConstantlyAttacking && !hasAirAttacked)
            {
                isAttacking = true;
                if (!isGrounded)
                {
                    rb.velocity = new Vector2(0, 0);
                    rb.gravityScale = 0;
                    StartCoroutine(DelayedResetGravityScale());
                    hasAirAttacked = true;
                }
                var enemiesInRange = new Collider2D[10];
                _ = Physics2D.OverlapCircleNonAlloc(attackOrigin.position, attackRadius, enemiesInRange, enemyLayer);
                foreach (var enemy in enemiesInRange)
                {
                    // make enemy take damage
                    if (enemy != null)
                    {
                        var damageable = enemy.GetComponent<IDamageable>();
                        damageable?.TakeDamage(playerDamage);
                    }
                }
                
                cooldownTimer = cooldownTime;
            }
        }
        else
        {
            isAttacking = false;
            cooldownTimer -= Time.deltaTime;
        }
    }

    private IEnumerator DelayedResetGravityScale()
    {
        yield return new WaitForSeconds(0.5f);
        rb.gravityScale = gravityScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }

    private void HandleJumping()
    {
        if (rb.velocity.y != 0)
        {
            rb.velocity -= gravityVector * (jumpDegrader * Time.deltaTime);
        }
    }

    private void HandleRunning()
    {
        var moveDistance = moveSpeed * Time.deltaTime;
        var moveDirX = new Vector3(moveDir.x, 0, 0);
        transform.position += moveDistance * moveDirX;
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - damage, 0, maxHealthPoint);
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }

    public Vector3 GetMoveDirection()
    {
        return moveDir;
    }

    public Vector3 GetVelocity()
    {
        return new Vector3(isRunning ? moveSpeed : 0, rb.velocity.y, 0);
    }

    public Vector3 GetPosition()
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public bool IsConstantlyAttacking()
    {
        return isConstantlyAttacking;
    }

    // public bool IsGroundedByRaycast() {
    //     // origin at the bottom of the collider
    //     Vector2 origin = new Vector2(transform.position.x, GetComponent<BoxCollider2D>().bounds.min.y);
    //     RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
    //     return hit.collider != null;
    // }

    public bool IsHit()
    {
        return false; // Placeholder for hit detection logic
    }

    public bool IsDeadHit()
    {
        return false; // Placeholder for dead hit detection logic
    }
}