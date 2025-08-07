using System.Linq;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarSecondStageRest : BossStarBaseStage
    {
        private float restTimer;
        private float initialHealth;
        private Transform targetedTransform;

        public BossStarSecondStageRest(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.SetActive(false);
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

            Wander();

            if (ShouldAttack())
            {
                BossStar.SetActive(true);
                Attack();
                BossStar.SetActive(false);
            }
            
            var enemiesToUnregister = BossStar.AliveEnemies.Where(aliveEnemy => aliveEnemy.CurrentHealth == 0).ToList();
            foreach (var aliveEnemy in enemiesToUnregister)
            {
                BossStar.UnregisterEnemy(aliveEnemy);
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
        
        private void Wander()
        {
            if (targetedTransform == null)
            {
                targetedTransform = BossStar.FleePositions[Random.Range(0, BossStar.FleePositions.Length)];
            }
            else if (BossStar.transform.position != targetedTransform.position)
            {
                BossStar.transform.position = Vector3.MoveTowards(
                    BossStar.transform.position,
                    targetedTransform.position,
                    BossStar.WanderSpeed * Time.deltaTime);
            }
            else
            {
                var nextPositionsCount = BossStar.FleePositions.Count(t => t != targetedTransform);
                var randomIndex = Random.Range(0, nextPositionsCount);
                targetedTransform = BossStar.FleePositions.Where(t => t != targetedTransform).ElementAt(randomIndex);
            }
        }
    }
}