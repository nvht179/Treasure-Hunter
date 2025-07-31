using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.MPE;
using UnityEngine;

public class CrabbyVisual : MonoBehaviour
{
    [SerializeField] private Crabby crabby;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    private static readonly int YVelocity = Animator.StringToHash("YVelocity");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int IsCharging = Animator.StringToHash("IsCharging"); // do not use this for anticipation animation
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int Charging = Animator.StringToHash("Charging");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int DeadHit = Animator.StringToHash("DeadHit");

    private bool playOnce;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        crabby.OnAttack += CrabbyOnAttack;
        crabby.OnDamageTaken += CrabbyOnDamageTaken;
    }

    private void CrabbyOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
    {
        var healthNormalized = e.CurrentHealth / e.MaxHealth;
        animator.SetTrigger(healthNormalized == 0f ? DeadHit : Hit);
    }

    private void CrabbyOnAttack(object sender, EventArgs e)
    {
        animator.SetTrigger(Attack);
    }

    private void Update()
    {
        HandleFlipX();
        var isJumping = !crabby.IsGrounded();
        var isCharging = crabby.GetState() == Crabby.CrabbyState.Charging;
        var isRecharging = crabby.GetState() == Crabby.CrabbyState.Recharging;
        if (isCharging && !playOnce)
        {
            animator.SetTrigger(Charging);
            playOnce = true;
        }
        else if (!isCharging)
        {
            playOnce = false;
        }

        var velocity = crabby.GetVelocity();
        animator.SetBool(IsJumping, isJumping);
        animator.SetBool(IsCharging, isCharging || isRecharging);
        animator.SetFloat(XVelocity, math.abs(velocity.x));
        animator.SetFloat(YVelocity, math.abs(velocity.y));
    }

    private void HandleFlipX()
    {
        spriteRenderer.flipX = crabby.GetMoveDirection().x switch
        {
            > 0f => true,
            < 0f => false,
            _ => spriteRenderer.flipX
        };
    }

    // get the remaining time until the hit animation finishes
    // flawed
    public float GetCurrentRemainingTime()
    {
        var currentState = animator.GetCurrentAnimatorStateInfo(0);

        var normalizedTime = currentState.normalizedTime;
        var timeInSeconds = normalizedTime * currentState.length;
        return timeInSeconds;
    }
}