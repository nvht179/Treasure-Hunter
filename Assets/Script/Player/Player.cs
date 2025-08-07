using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Player Stats")]
    [SerializeField] private float basePlayerDamage = 5f;
    [SerializeField] private float baseCritChance = 0.1f; // 15% crit chance
    [SerializeField] private float baseCritMultiplier = 2f; // 2x damage on crit
    [SerializeField] private float baseHealth = 100f;
    [SerializeField] private float baseHealthRestoreRate = 0.1f;
    [SerializeField] private float baseStamina = 100f;
    [SerializeField] private float baseStaminaRestoreRate = 0.5f;
    [SerializeField] private float baseMoveSpeed = 5f;

    [Header("Jump System")]
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpDegrader;

    [Header("References")]
    [SerializeField] private FlyingObjectSO flyingSwordSO;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask interactiveObjectLayer;

    [Header("Attack")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Transform playerPivot;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackCooldownTime;
    [SerializeField] private float attackAlternateCooldownTime;
    [SerializeField] private float attackAlternateStaminaCost;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackDuration;

    [Header("Inventory")]
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ShopUI shopUI;

    private const float GroundCheckRadius = 0.05f;
    private const float AirAttackTime = 0.5f;

    public event EventHandler<OnSelectedObjectChangedEventArgs> OnSelectedObjectChanged;
    public class OnSelectedObjectChangedEventArgs : EventArgs
    {
        public IInteractiveObject SelectedObject;
    }

    // Sound effects
    public event EventHandler OnMoveHorizontal;
    public event EventHandler OnJump;
    public event EventHandler OnJumpLand;
    public event EventHandler OnAttack;
    public event EventHandler OnAirAttack;
    public event EventHandler OnAttackAlternate;
    public event EventHandler OnDead;

    public event EventHandler OnGreenPotionFail;
    public event EventHandler OnGreenPotionSuccess;
    public event EventHandler OnBluePotionUsed;
    public event EventHandler OnHealthPotionUsed;
    public event EventHandler OnKeyCollected;

    public event EventHandler OnHealthChanged; // For refactor
    public event EventHandler OnDestroyed;
    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;
    public event EventHandler<OnStaminaUsedEventArgs> OnStaminaUsed;
    public event EventHandler OnNeedKey;
    public event Action OnWon;
    public class OnStaminaUsedEventArgs : EventArgs
    {
        public float CurrentStamina;
        public float MaxStamina;
    }
    public event EventHandler<OnGoldChangedEventArgs> OnGoldChanged;
    public class OnGoldChangedEventArgs : EventArgs
    {
        public int CurrentGold;
        public int ChangeAmount;
    }

    // Move and Jump
    private float gravityScale;
    private bool isGrounded;
    private Vector3 moveVector;
    private int moveDirection;
    private Vector2 gravityVector;
    public bool IsFacingRight { get; set; }

    // Attack
    private bool isConstantlyAttacking;
    private bool isAttacking;
    private bool isDamaged;
    private bool hasAirAttacked;
    private float attackCooldownTimer;
    private float attackAlternateCooldownTimer;
    private float airAttackTimer;
    
    // Hit
    private Vector2 knockbackVelocity;
    private float knockbackTimer;
    
    private Rigidbody2D rb;

    private Inventory inventory;
    private IInteractiveObject selectedObject;
    [SerializeField] private int money; // TODO: remove SerializeField later
    [SerializeField] private float footstepInterval = 0.4f;
    private float footstepTimer;

    // Attributes
    public HealthSystem HealthSystem { get; private set; }
    public StaminaSystem StaminaSystem { get; private set; }
    public MoveSpeedSystem MoveSpeedSystem { get; private set; }
    public GoldBonusSystem GoldBonusSystem { get; private set; }
    public DamageSystem DamageSystem { get; private set; }
    public DamageReceivedSystem DamageReceivedSystem { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        IsFacingRight = true;
        isConstantlyAttacking = false;
        hasAirAttacked = false;
        gravityScale = rb.gravityScale;
        attackAlternateCooldownTimer = attackAlternateCooldownTime;

        inventory = new Inventory();
        inventoryUI.SetInventory(inventory);
        money = 200; // TODO: Initial money for testing purposes

        HealthSystem = new HealthSystem(baseHealth, baseHealthRestoreRate);
        StaminaSystem = new StaminaSystem(baseStamina, baseStaminaRestoreRate);
        MoveSpeedSystem = new MoveSpeedSystem(baseMoveSpeed);
        GoldBonusSystem = new GoldBonusSystem();
        DamageSystem = new DamageSystem(basePlayerDamage, baseCritChance, baseCritMultiplier);
        DamageReceivedSystem = new DamageReceivedSystem();
    }

    private void HealthSystem_OnDeath()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    private void StaminaSystem_OnStaminaChanged(float currentStamina, float maxStamina)
    {
        OnStaminaUsed?.Invoke(this, new OnStaminaUsedEventArgs
        {
            CurrentStamina = currentStamina,
            MaxStamina = maxStamina
        });
    }

    private void Start()
    {
        GameInput.Instance.OnJumpAction += PlayerOnJump;
        GameInput.Instance.OnAttackAction += PlayerOnAttack;
        GameInput.Instance.OnAttackAlternateAction += PlayerOnAttackAlternate;
        GameInput.Instance.OnInteractAction += PlayerOnInteract;

        HealthSystem.OnDeath += HealthSystem_OnDeath;

        StaminaSystem.OnStaminaChanged += StaminaSystem_OnStaminaChanged;

        shopUI.OnItemBuy += ShopUI_OnItemBuy;

        gravityVector = new Vector2(0, -Physics2D.gravity.y);
    }

    private void ShopUI_OnItemBuy(object sender, ShopUI.OnItemBuyEventArgs e) {
        BuyItem(e.item.itemSO.buyPrice);
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
            OnJump?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayerOnAttack(object sender, EventArgs e)
    {
        isConstantlyAttacking = !isConstantlyAttacking;

    }

    private void PlayerOnAttackAlternate(object sender, EventArgs e)
    {
        if (attackAlternateCooldownTimer < 0 && StaminaSystem.CanUse(attackAlternateStaminaCost))
        {
            var flyingSwordTransform = Instantiate(flyingSwordSO.prefab, attackOrigin.position, attackOrigin.rotation);
            flyingSwordTransform.SetParent(transform);
            if (flyingSwordTransform.TryGetComponent<FlyingSword>(out var flyingSword))
            {
                flyingSword.Direction = IsFacingRight ? 1 : -1;
                flyingSword.SetDamage(DamageSystem.CalculateOutgoingDamage(basePlayerDamage, out bool isCrit));
            }

            attackAlternateCooldownTimer = attackAlternateCooldownTime;
            StaminaSystem.Use(attackAlternateStaminaCost);

            OnAttackAlternate?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        HandleUpdateState();
        HandleInteractions();

        HealthSystem.Regenerate();
        StaminaSystem.Regenerate();
        DamageSystem.Tick();
        DamageReceivedSystem.Tick();
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

        // TODO: modularize this
        if (moveVector.x != 0 && isGrounded)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                OnMoveHorizontal?.Invoke(this, EventArgs.Empty);
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f; // reset when not moving
        }
    }

    private void HandleInteractions()
    {
        float interactionDistance = 1f;
        Vector2 moveDir = IsFacingRight ? Vector2.right : Vector2.left;

        RaycastHit2D raycastHit = Physics2D.Raycast(attackOrigin.position, moveDir, interactionDistance, interactiveObjectLayer);
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

    private void SetSelectedObject(IInteractiveObject selectedObject)
    {
        this.selectedObject = selectedObject;
        OnSelectedObjectChanged?.Invoke(this, new OnSelectedObjectChangedEventArgs
        {
            SelectedObject = this.selectedObject
        });
    }

    private void FixedUpdate()
    {
        
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            rb.velocity = knockbackVelocity;
            return; // Skip normal movement
        }

        // horizontal movement
        var velocityX = moveVector.x == 0 ? 0 : MoveSpeedSystem.GetCurrentSpeed() * moveDirection;

        if (airAttackTimer > 0)
        {
            velocityX = 0;
        }

        // vertical movement
        var velocityY = rb.velocity.y;

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
                    OnAirAttack?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Player is attacking in the air");
                }
                else
                {
                    OnAirAttack?.Invoke(this, EventArgs.Empty);
                    Debug.Log("Player is attacking on the ground");
                }

                var enemiesInRange = new Collider2D[10];
                _ = Physics2D.OverlapCircleNonAlloc(attackOrigin.position, attackRadius, enemiesInRange, enemyLayer);
                foreach (var enemy in enemiesInRange)
                {
                    // make enemy take damage
                    if (enemy != null)
                    {
                        var damageable = enemy.GetComponent<IDamageable>();
                        var offenderInfo = new IDamageable.DamageInfo
                        {
                            Damage = DamageSystem.CalculateOutgoingDamage(basePlayerDamage, out bool isCrit),
                        };
                        damageable?.TakeDamage(offenderInfo);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemWorld>(out var itemWorld))
        {
            ItemSO itemSO = itemWorld.GetItem().itemSO;
            if(itemSO is ResourceItemSO resourceItemSO)
            {
                HandleResourceCollected(itemWorld, resourceItemSO);
                if (itemSO.itemType == ItemType.GoldenKey)
                {
                    inventory.AddItem(itemWorld.GetItem());
                    OnKeyCollected?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                inventory.AddItem(itemWorld.GetItem());
            }
        }
    }

    private void HandleResourceCollected(ItemWorld resourceItemWorld, ResourceItemSO resourceItemSO)
    {
        int collectedAmount = GoldBonusSystem.ApplyGoldBonus(resourceItemSO.value * resourceItemWorld.GetItem().quantity);
        OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs
        {
            CurrentGold = money,
            ChangeAmount = collectedAmount
        });

        money += collectedAmount;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }

    public void TakeDamage(IDamageable.DamageInfo offenderInfo)
    {
        offenderInfo.Damage = DamageReceivedSystem.CalculateReceivedDamage(offenderInfo.Damage);
        HealthSystem.TakeDamage(offenderInfo.Damage);

        var knockbackDir = offenderInfo.Velocity.normalized;
        knockbackVelocity = knockbackDir * knockbackForce;
        knockbackTimer = knockbackDuration;
    }

    public void ActivateNeedKeyDialog()
    {
        OnNeedKey?.Invoke(this, EventArgs.Empty);
    }

    public void BuyItem(int price)
    {
        OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs
        {
            CurrentGold = money,
            ChangeAmount = -price
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

    public bool HasKey() 
    {
        return inventory.HasKey();
    }

    public InventoryUI GetInventoryUI()
    {
        return inventoryUI;
    }
}