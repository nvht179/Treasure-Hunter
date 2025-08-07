using System;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarDead: BossStarBaseStage
    {
        private float deadShowTimer;
        
        public BossStarDead(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            deadShowTimer = BossStarContext.DeadShowTime;
        }

        public override void UpdateState()
        {
            deadShowTimer -= Time.deltaTime;
            if (deadShowTimer < 0)
            {
                BossStar.SelfDestroy();
            }
        }

        public override void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
        }
    }
}