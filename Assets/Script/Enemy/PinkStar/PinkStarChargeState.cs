using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarChargeState : PinkStarBaseState
    {
        private float chargeTimer;

        public PinkStarChargeState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStarStateManager.PinkStarState.Charge;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
            chargeTimer = PinkStar.ChargeTime;
        }

        public override void UpdateState()
        {
            chargeTimer -= Time.deltaTime;

            if (PinkStar.IsWallAhead() && !PinkStar.HasContinuousPathToPlayer())
            {
                PinkStar.SwitchState(PinkStar.WaitState);
            }

            if (chargeTimer < 0)
            {
                PinkStar.SwitchState(PinkStar.AttackState);
            }
        }
    }
}