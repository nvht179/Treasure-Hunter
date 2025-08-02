using System;

namespace Script.Enemy.BossStar
{
    public class BossStarFirstStageRest: BossStarBaseStage
    {
        public BossStarFirstStageRest(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
            throw new NotImplementedException();
        }

        public override void UpdateState()
        {
            throw new NotImplementedException();
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
        }
    }
}