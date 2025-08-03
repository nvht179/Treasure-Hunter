using System;
using Unity.Mathematics;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform attackOrigin;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 direction;
    private Vector3 initialAttackOriginPosition;

    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsConstantlyAttacking = Animator.StringToHash("IsConstantlyAttacking");
    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    private static readonly int YVelocity = Animator.StringToHash("YVelocity");
    private static readonly int Hit = Animator.StringToHash("Hit");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialAttackOriginPosition = attackOrigin.localPosition;
    }

    private void Start()
    {
        player.HealthSystem.OnDamageReceived += HealthSystemOnDamageReceived;
    }

    private void HealthSystemOnDamageReceived(object sender, EventArgs e)
    {
        animator.SetTrigger(Hit);
    }

    private void Update()
    {
        // update sprite
        direction = player.GetMoveDirection();
        HandleFlipX();

        // handle animation
        var isJumping = !player.IsGrounded();
        var isAttacking = player.IsAttacking();
        var isConstantlyAttacking = player.IsConstantlyAttacking();
        var velocity = player.GetVelocity();

        animator.SetBool(IsJumping, isJumping);
        animator.SetBool(IsAttacking, isAttacking);
        animator.SetBool(IsConstantlyAttacking, isConstantlyAttacking);
        animator.SetFloat(XVelocity, math.abs(velocity.x));
        animator.SetFloat(YVelocity, math.abs(velocity.y));
    }

    private void HandleFlipX()
    {
        switch (direction.x)
        {
            case > 0f:
                spriteRenderer.flipX = false;
                attackOrigin.localPosition = initialAttackOriginPosition;
                player.IsFacingRight = true;
                break;
            case < 0f:
                spriteRenderer.flipX = true;
                attackOrigin.localPosition = new Vector3(-initialAttackOriginPosition.x, initialAttackOriginPosition.y,
                    initialAttackOriginPosition.z);
                player.IsFacingRight = false;
                break;
        }
    }
}