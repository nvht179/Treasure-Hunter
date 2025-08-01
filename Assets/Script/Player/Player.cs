using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Jump System")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpDegrader;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform playerPivot;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask interactiveObjectLayer;
    [SerializeField] private FlyingObjectSO flyingSwordSO;

    [Header("Attack")]
    [SerializeField] private int playerDamage;
    [SerializeField] private float attackCooldownTime;
    [SerializeField] private float attackAlternateCooldownTime;
    [SerializeField] private float attackAlternateStaminaCost;
    [SerializeField] private float maxHealthPoint;
    [SerializeField] private float maxStamina;

    [Header("Inventory")]
    [SerializeField] private UI_Inventory uiInventory;

    private const float GroundCheckRadius = 0.05f;
    private const float AirAttackTime = 0.5f;

    public event EventHandler<OnSelectedObjectChangedEventArgs> OnSelectedObjectChanged;
    public class OnSelectedObjectChangedEventArgs : EventArgs
    {
        public IInteractiveObject selectedObject;
    }

    public event EventHandler OnJump;

    public event EventHandler OnDestroyed;
    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;
    public event EventHandler<OnStaminaUsedEventArgs> OnStaminaUsed;
    public event Action OnDied;
    public event Action OnWon;
    public class OnStaminaUsedEventArgs : EventArgs
    {
        public float CurrentStamina;
        public float MaxStamina;
    }
    public event EventHandler<OnGoldChangedEventArgs> OnGoldChanged;
    public class OnGoldChangedEventArgs : EventArgs
    {
        public int currentGold;
        public int changeAmount;
    }

    private float gravityScale;
    private float currentHealthPoint;
    private float currentStamina;
    private bool isGrounded;
    private bool isConstantlyAttacking;
    private bool isAttacking;
    private bool isDamaged;
    private bool hasAirAttacked;
    private float attackCooldownTimer;
    private float attackAlternateCooldownTimer;
    private float airAttackTimer;
    private Vector3 moveVector;
    private int moveDirection;
    private Vector2 gravityVector;
    public bool IsFacingRight { get; set; }
    private Rigidbody2D rb;

    private Inventory inventory;
    private IInteractiveObject selectedObject;
    private int money;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        IsFacingRight = true;
        isConstantlyAttacking = false;
        hasAirAttacked = false;
        currentHealthPoint = maxHealthPoint;
        currentStamina = maxStamina;
        gravityScale = rb.gravityScale;
        attackAlternateCooldownTimer = attackAlternateCooldownTime;

        inventory = new Inventory(UseItem, WearItem, DropItem);
        uiInventory.SetInventory(inventory);
        money = 0;
    }

    private void Start()
    {
        GameInput.Instance.OnJumpAction += PlayerOnJump;
        GameInput.Instance.OnAttackAction += PlayerOnAttack;
        GameInput.Instance.OnAttackAlternateAction += PlayerOnAttackAlternate;
        GameInput.Instance.OnInteractAction += PlayerOnInteract;

        gravityVector = new Vector2(0, -Physics2D.gravity.y);

    }

    private void OnDestroy()
    {
        GameInput.Instance.OnJumpAction -= PlayerOnJump;
        GameInput.Instance.OnAttackAction -= PlayerOnAttack;
        GameInput.Instance.OnAttackAlternateAction -= PlayerOnAttackAlternate;
        GameInput.Instance.OnInteractAction -= PlayerOnInteract;
        OnDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerOnInteract(object sender, EventArgs e)
    {
        selectedObject?.Interact(this);
    }

    private void PlayerOnJump(object sender, EventArgs e)
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            OnJump.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayerOnAttack(object sender, EventArgs e)
    {
        isConstantlyAttacking = !isConstantlyAttacking;
    }

    private void PlayerOnAttackAlternate(object sender, EventArgs e)
    {
        if (attackAlternateCooldownTimer < 0 && currentStamina > 0)
        {
            var flyingSwordTransform = Instantiate(flyingSwordSO.prefab, attackOrigin.position, attackOrigin.rotation);
            flyingSwordTransform.SetParent(transform);
            if (flyingSwordTransform.TryGetComponent<FlyingSword>(out var flyingSword))
            {
                flyingSword.Direction = IsFacingRight ? 1 : -1;
            }

            attackAlternateCooldownTimer = attackAlternateCooldownTime;
            currentStamina = Mathf.Clamp(currentStamina - attackAlternateStaminaCost, 0, maxStamina);
            OnStaminaUsed?.Invoke(this, new OnStaminaUsedEventArgs
            {
                CurrentStamina = currentStamina,
                MaxStamina = maxStamina
            });
        }
    }

    private void Update()
    {
        HandleUpdateState();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        float interactionDistance = 1.5f;
        Vector2 moveDir = moveDirection == -1 ? Vector2.left : Vector2.right;

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, moveDir, interactionDistance, interactiveObjectLayer);
        if (raycastHit.collider != null)
        {
            if (raycastHit.collider.TryGetComponent(out IInteractiveObject interactiveObject))
            {
                if (interactiveObject != selectedObject)
                {
                    SetSelectedObject(interactiveObject);
                }
            }
            else
            {
                SetSelectedObject(null);
            }
        }
        else
        {
            SetSelectedObject(null);
        }
    }

    private void HandleUpdateState()
    {
        // handle input
        var inputVector = GameInput.Instance.GetMovementVectorNormalized();
        moveVector = (Vector3)inputVector;

        HandleAttack();

        // update states
        moveDirection = moveVector.x > 0 ? 1 : -1;
        attackAlternateCooldownTimer -= Time.deltaTime;
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

    private void SetSelectedObject(IInteractiveObject selectedObject)
    {
        this.selectedObject = selectedObject;
        OnSelectedObjectChanged?.Invoke(this, new OnSelectedObjectChangedEventArgs
        {
            selectedObject = this.selectedObject
        });
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


    private void HandleAttack()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemWorld>(out var itemWorld))
        {
            OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs
            {
                currentGold = money,
                changeAmount = itemWorld.GetItem().quantity
            });

            money += itemWorld.GetItem().quantity;
            itemWorld.DestroySelf();
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

        if (currentHealthPoint <= 0)
        {
            OnDied?.Invoke();
        }
    }

    public void BuyItem(int price)
    {
        OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs
        {
            currentGold = money,
            changeAmount = -price
        });
        money -= price;
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    public int GetMoney()
    {
        return money;
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

    private void WearItem(Item item)
    {
        switch(item.itemSO.itemType)
        {
            case ItemType.BlueDiamond:
                BlueDiamond();
                break;
            case ItemType.GoldenSkull:
                GoldenSkull();
                break;
            case ItemType.GreenDiamond:
                GreenDiamond();
                break;
            case ItemType.RedDiamond:
                RedDiamond();
                break;
        }
    }

    private void DropItem(Item item)
    {
        switch(item.itemSO.itemType)
        {
            case ItemType.BlueDiamond:
                DropBlueDiamond();
                break;
            case ItemType.GoldenSkull:
                DropGoldenSkull();
                break;
            case ItemType.GreenDiamond:
                DropGreenDiamond();
                break;
            case ItemType.RedDiamond:
                DropRedDiamond();
                break;
        }
    }

    private void UseItem(Item item)
    {
        switch(item.itemSO.itemType)
        {
            case ItemType.BluePotion:
                BluePotion();
                break;
            case ItemType.GreenPotion:
                GreenPotion();
                break;
            case ItemType.HealthPotion:
                HealthPotion();
                break;
        }
    }

    private void BlueDiamond()
    {
        maxStamina += 10;
        currentStamina += 10;
        OnStaminaUsed?.Invoke(this, new OnStaminaUsedEventArgs
        {
            CurrentStamina = currentStamina,
            MaxStamina = maxStamina
        });
    }

    private void BluePotion()
    {
        // TODO: Restore stamina rate in 5s
    }

    private void GoldenSkull()
    {
        maxHealthPoint += 50;
        playerDamage += 5;
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }

    private void GreenPotion()
    {
        int chance = UnityEngine.Random.Range(0, 100);
        if(chance < 50)
        {
            currentHealthPoint /= 2;
        }
        else
        {
            currentHealthPoint = maxHealthPoint;
        }

        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }

    private void GreenDiamond()
    {
        currentHealthPoint = 1;
        maxHealthPoint = 1;
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
        playerDamage = 99999;
    }

    private void RedDiamond()
    {
        maxHealthPoint += 10;
        currentHealthPoint += 10;
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }

    private void HealthPotion()
    {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint + 10, 0, maxHealthPoint);
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }

    private void DropBlueDiamond()
    {
        maxStamina -= 10;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        OnStaminaUsed?.Invoke(this, new OnStaminaUsedEventArgs
        {
            CurrentStamina = currentStamina,
            MaxStamina = maxStamina
        });
    }

    private void DropGoldenSkull()
    {
        maxHealthPoint -= 50;
        playerDamage -= 5;
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0, maxHealthPoint);
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }

    private void DropGreenDiamond()
    {
        currentHealthPoint = 100;
        maxHealthPoint = 100;
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
        playerDamage = 5;
    }

    private void DropRedDiamond()
    {
        maxHealthPoint -= 10;
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0, maxHealthPoint);
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs
        {
            CurrentHealth = currentHealthPoint,
            MaxHealth = maxHealthPoint
        });
    }
}