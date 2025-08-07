using System.Collections;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarHitVisual : MonoBehaviour
    {
        [SerializeField] private BossStarContext bossStar;
        private SpriteRenderer spriteRenderer;
        private Animator animator;
        
        private static readonly int IsActive = Animator.StringToHash("IsActive");

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
            if (e.CurrentHealth != 0)
            {
                StartCoroutine(BlinkCharacter(3, 0.1f));
            }
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
            var velocity = bossStar.GetVelocity();
            var isActive = bossStar.IsActive();

            animator.SetBool(IsActive, isActive || velocity != Vector2.zero);
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