using System;
using Unity.Mathematics;
using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarVisual : MonoBehaviour
    {
        [SerializeField] private PinkStarStateManager pinkStar;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private static readonly int XVelocity = Animator.StringToHash("XVelocity");
        private static readonly int YVelocity = Animator.StringToHash("YVelocity");
        private static readonly int Attack = Animator.StringToHash("Attack");

        private static readonly int
            IsCharging = Animator.StringToHash("IsCharging"); // do not use this for anticipation animation

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

        // private void Start()
        // {
        //     pinkStar.OnAttack += PinkStarOnAttack;
        //     pinkStar.OnDamageTaken += PinkStarOnDamageTaken;
        // }

        private void PinkStarOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
        {
            var healthNormalized = e.CurrentHealth / e.MaxHealth;
            animator.SetTrigger(healthNormalized == 0f ? DeadHit : Hit);
        }

        private void PinkStarOnAttack(object sender, EventArgs e)
        {
            animator.SetTrigger(Attack);
        }

        private void Update()
        {
            HandleFlipX();
            var velocity = pinkStar.GetVelocity();
            var isJumping = !pinkStar.IsGrounded();
            var isCharging = pinkStar.GetCurrentState() == PinkStarStateManager.PinkStarState.Charge;
            var isRecharging = pinkStar.GetCurrentState() == PinkStarStateManager.PinkStarState.Recharge;

            if (isCharging && !playOnce)
            {
                animator.SetTrigger(Charging);
                playOnce = true;
            }
            else if (!isCharging)
            {
                playOnce = false;
            }

            animator.SetBool(IsJumping, isJumping);
            animator.SetBool(IsCharging, isCharging || isRecharging);
            animator.SetFloat(XVelocity, math.abs(velocity.x));
            animator.SetFloat(YVelocity, math.abs(velocity.y));
        }

        private void HandleFlipX()
        {
            spriteRenderer.flipX = pinkStar.MoveDirection switch
            {
                1 => true,
                -1 => false,
                _ => spriteRenderer.flipX
            };
        }
    }
}