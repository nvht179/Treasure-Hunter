using System;
using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarFirstStageActive : BossStarBaseStage
    {
        private float firstStageActiveTimer;
        public BossStarFirstStageActive(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.transform.position = Vector3.zero;
            firstStageActiveTimer = BossStar.FirstStageActiveTime;
        }

        public override void UpdateState()
        {
            firstStageActiveTimer -= Time.deltaTime;

            if (firstStageActiveTimer < 0)
            {
                BossStar.SwitchState(BossStar.FirstStageRest);
            }
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
        }
    }
}