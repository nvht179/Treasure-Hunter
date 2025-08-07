using UnityEngine;

namespace Script.Enemy.BossStar
{
    public class BossStarThirdStageTransition : BossStarBaseStage
    {
        /*
            transition between second stage and third stage that
            moves the boss to the central and makes it invincible
        */
        public BossStarThirdStageTransition(BossStarContext context) : base(context)
        {
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            if (BossStar.transform.position != BossStar.CentralTransform.position)
            {
                BossStar.transform.position = Vector3.MoveTowards(
                    BossStar.transform.position,
                    BossStar.CentralTransform.position,
                    BossStar.FleeSpeed * Time.deltaTime);
            }
            else
            {
                BossStar.SwitchState(BossStar.ThirdStageActive);
            }
        }

        public override void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            // invincible - does not take damage
        }
    }
}