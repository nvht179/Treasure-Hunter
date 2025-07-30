using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Jump System")]
    [SerializeField] private float jumpPower;
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
    [SerializeField] private float attackCooldownTime;
    [SerializeField] private float maxHealthPoint;

    [Header("Inventory")]
    [SerializeField] private UI_Inventory uiInventory;

    private const float GroundCheckRadius = 0.05f;
    private const float AirAttackTime = 0.5f;

    public event EventHandler OnDestroyed;
    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;

    private float gravityScale;
    private float currentHealthPoint;
    private bool isGrounded;
    private bool isConstantlyAttacking;
    private bool isAttacking;
    private bool isDamaged;
    private bool hasAirAttacked;
    private float attackCooldownTimer;
    private float airAttackTimer;
    private Vector3 moveVector;
    private int moveDirection;
    private Vector2 gravityVector;
    public bool IsFacingRight { get; set; }
    private Rigidbody2D rb;

    private Inventory inventory;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        IsFacingRight = true;
        isConstantlyAttacking = false;
        hasAirAttacked = false;
        currentHealthPoint = maxHealthPoint;
        gravityScale = rb.gravityScale;

        //inventory = new Inventory();
        //uiInventory.SetInventory(inventory);
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
        moveVector = new Vector3(inputVector.x, inputVector.y, 0);

        HandleAttacking();

        // update states
        moveDirection = moveVector.x > 0 ? 1 : -1;
        airAttackTimer -= Time.deltaTime;
        if (isAttacking && !isGrounded)
        {
            airAttackTimer = AirAttackTime;
        }

        isGrounded = Physics2D.OverlapCircle(playerPivot.position, GroundCheckRadius, groundLayer);
        if (isGrounded)
        {
            hasAirAttacked = false;
        }
    }

    private void FixedUpdate()
    {
        var currentVelocity = rb.velocity;

        // horizontal movement
        var velocityX = moveVector.x == 0 ? 0 : moveSpeed * moveDirection;

        if (airAttackTimer > 0)
        {
            velocityX = 0;
        }

        // vertical movement
        var velocityY = currentVelocity.y;

        if (velocityY != 0)
        {
            velocityY -= gravityVector.y * jumpDegrader * Time.fixedDeltaTime;
        }

        rb.velocity = new Vector2(velocityX, velocityY);
    }


    private void HandleAttacking()
    {
        if (attackCooldownTimer <= 0)
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

                attackCooldownTimer = attackCooldownTime;
            }
        }
        else
        {
            isAttacking = false;
            attackCooldownTimer -= Time.deltaTime;
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

    // private void HandleRunning()
    // {
    //     var moveDistance = moveSpeed * Time.deltaTime;
    //     var moveDirX = new Vector3(moveVector.x, 0, 0);
    //     transform.position += moveDistance * moveDirX;
    // }

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
        return moveVector;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z);
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
}