using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarSecondStageTransition : BossStarBaseStage
    {
        // transition between first stage and second stage that makes the boss invincible
        private float invincibleTimer;

        public BossStarSecondStageTransition(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            invincibleTimer = BossStar.InvincibleDuration;
            BossStar.SetInvincible(true);
        }

        public override void UpdateState()
        {
            invincibleTimer -= Time.deltaTime;

            if (invincibleTimer < 0)
            {
                BossStar.SwitchState(BossStar.SecondStageActive);
                BossStar.SetInvincible(false);
            }
        }

        public override void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            // invincible - does not take damage
        }
    }
}