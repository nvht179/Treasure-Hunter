using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarHitVisual : MonoBehaviour
    {
        [SerializeField] private PinkStarStateManager pinkStar;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private bool isCharging;
        private bool isJumping;
        private bool isAttacking;
        private bool isRecharging;

        private static readonly int XVelocity = Animator.StringToHash("XVelocity");
        private static readonly int YVelocity = Animator.StringToHash("YVelocity");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

        private static readonly int
            IsCharging = Animator.StringToHash("IsCharging"); // do not use this for anticipation animation

        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsRecharging = Animator.StringToHash("IsRecharging");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int DeadHit = Animator.StringToHash("DeadHit");

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            spriteRenderer.enabled = false;
        }

        private void Start()
        {
            pinkStar.OnDamageTaken += PinkStarOnDamageTaken;
        }

        private void PinkStarOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
        {
            if (e.CurrentHealth != 0)
            {
                StartCoroutine(BlinkCharacter(3, 0.1f));
                return;
            }

            animator.SetTrigger(e.CurrentHealth == 0f ? DeadHit : Hit);
        }

        private IEnumerator BlinkCharacter(int times, float interval)
        {
            for (var i = 0; i < times; i++)
            {
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(interval);
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(interval);
            }
        }

        private void Update()
        {
            HandleFlipX();
            var velocity = pinkStar.GetVelocity();
            isJumping = !pinkStar.IsGrounded();
            isAttacking = pinkStar.GetCurrentState() == PinkStarStateManager.PinkStarState.Attack;
            isRecharging = pinkStar.GetCurrentState() == PinkStarStateManager.PinkStarState.Recharge;
            isCharging = pinkStar.GetCurrentState() == PinkStarStateManager.PinkStarState.Charge;

            animator.SetBool(IsJumping, isJumping);
            animator.SetBool(IsAttacking, isAttacking);
            animator.SetBool(IsCharging, isCharging);
            animator.SetBool(IsRecharging, isRecharging);
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