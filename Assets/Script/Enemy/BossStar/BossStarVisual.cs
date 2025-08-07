using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarVisual : MonoBehaviour
    {
        [SerializeField] private BossStarContext bossStar;
        [SerializeField] private Transform shield;

        private SpriteRenderer spriteRenderer;
        private Animator animator;

        private static readonly int IsActive = Animator.StringToHash("IsActive");
        private static readonly int DeadHit = Animator.StringToHash("DeadHit");

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            bossStar.OnDamageTaken += BossStarOnDamageTaken;
        }

        private void BossStarOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
        {
            if (e.CurrentHealth == 0)
            {
                animator.SetTrigger(DeadHit);
            }
        }

        private void Update()
        {
            HandleFlipX();
            var velocity = bossStar.GetVelocity();
            var isActive = bossStar.IsActive();
            var isInvincible = bossStar.IsInvincible();

            animator.SetBool(IsActive, isActive || velocity != Vector2.zero);
            if(isInvincible)
            {
                shield.gameObject.SetActive(true);
            }
            else
            {
                shield.gameObject.SetActive(false);
            }
        }

        private void HandleFlipX()
        {
            spriteRenderer.flipX = bossStar.MoveDirection switch
            {
                1 => true,
                -1 => false,
                _ => spriteRenderer.flipX
            };
        }
    }
}