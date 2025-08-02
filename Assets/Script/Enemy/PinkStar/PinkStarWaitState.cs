using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarWaitState : PinkStarBaseState
    {
        public PinkStarWaitState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStarStateManager.PinkStarState.Wait;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
        }

        public override void UpdateState()
        {
            if (PinkStar.IsPlayerDetected())
            {
                if (PinkStar.HasContinuousPathToPlayer())
                {
                    PinkStar.SwitchState(PinkStar.ChargeState);
                }
            }
            else
            {
                PinkStar.SwitchState(PinkStar.WanderState);
            }
        }

        public override void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            base.TakeDamage(offenderInfo);
            PinkStar.SwitchState(PinkStar.HitState);
        }
    }
}