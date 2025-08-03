using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarSecondStageRest : BossStarBaseStage
    {
        private float restTimer;
        private float initialHealth;

        public BossStarSecondStageRest(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            restTimer = BossStar.SecondStageRestTime;
            initialHealth = BossStar.CurrentHealth;
        }

        public override void UpdateState()
        {
            restTimer -= Time.deltaTime;
            if (BossStar.CurrentHealth < BossStar.MaxHealth * BossStarContext.ThirdStageMaxHealthRatio)
            {
                BossStar.SwitchState(BossStar.ThirdStageActive);
                return;
            }

            if (restTimer < 0)
            {
                BossStar.SwitchState(BossStar.SecondStageActive);
                return;
            }

            if (ShouldAttack())
            {
                Attack();
            }
        }

        private void Attack()
        {
            var enemiesInRange = new Collider2D[10];
            _ = Physics2D.OverlapCircleNonAlloc(BossStar.Pivot.position, BossStar.AttackRadius, enemiesInRange, BossStar.PlayerLayer);
            foreach (var enemy in enemiesInRange)
            {
                if (enemy != null)
                {
                    var damageable = enemy.GetComponent<IDamageable>();
                    var offenderInfo = new IDamageable.DamageInfo
                    {
                        Damage = BossStar.AttackDamage,
                        Force = BossStar.KnockbackForce,
                        Velocity = (BossStar.Player.transform.position - BossStar.Pivot.position).normalized
                    };
                    damageable?.TakeDamage(offenderInfo);
                }
            }

            initialHealth = BossStar.CurrentHealth;
        }

        private bool ShouldAttack()
        {
            var healthLost = initialHealth - BossStar.CurrentHealth;
            var fleeThreshold = BossStar.MaxHealth * BossStarContext.FleePercentageMaxHealthThreshold;
            return healthLost > fleeThreshold;
        }
    }
}