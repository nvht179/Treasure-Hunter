using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform attackOrigin;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector3 direction;

    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsConstantlyAttacking = Animator.StringToHash("IsConstantlyAttacking");
    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    private static readonly int YVelocity = Animator.StringToHash("YVelocity");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        animator.SetFloat(XVelocity, velocity.x);
        animator.SetFloat(YVelocity, velocity.y);
    }

    private void HandleFlipX()
    {
        switch (direction.x)
        {
            case > 0f:
                spriteRenderer.flipX = false;
                attackOrigin.localPosition = new Vector3(math.abs(attackOrigin.localPosition.x), attackOrigin.localPosition.y,
                    attackOrigin.localPosition.z);
                player.IsFacingRight = true;
                break;
            case < 0f:
                attackOrigin.localPosition = new Vector3(-attackOrigin.localPosition.x, attackOrigin.localPosition.y,
                    attackOrigin.localPosition.z);
                spriteRenderer.flipX = true;
                player.IsFacingRight = false;
                break;
        }
    }
}