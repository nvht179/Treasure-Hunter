using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarFirstStageRest : BossStarBaseStage
    {
        private float restTimer;

        public BossStarFirstStageRest(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            BossStar.SetActive(false);
            restTimer = BossStar.FirstStageRestTime;
        }

        public override void UpdateState()
        {
            restTimer -= Time.deltaTime;

            if (BossStar.CurrentHealth < BossStar.MaxHealth * BossStarContext.SecondStageMaxHealthRatio)
            {
                BossStar.SwitchState(BossStar.SecondStageActive);
            }
            else if (restTimer < 0)
            {
                BossStar.SwitchState(BossStar.FirstStageActive);
            }
        }
    }
}