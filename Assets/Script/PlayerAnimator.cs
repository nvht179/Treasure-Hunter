using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsConstantlyAttacking = Animator.StringToHash("IsConstantlyAttacking");
    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    private static readonly int YVelocity = Animator.StringToHash("YVelocity");

    [SerializeField] private Player player;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
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
}
