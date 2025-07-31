using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarWanderState : PinkStarBaseState
    {
        public PinkStarWanderState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStarStateManager.PinkStarState.Wander;
            Rb.velocity = new Vector2(PinkStar.MoveSpeed * PinkStar.MoveDirection, Rb.velocity.y);
        }

        public override void UpdateState()
        {
            if (!PinkStar.IsGroundAhead() || PinkStar.IsWallAhead())
            {
                PinkStar.MoveDirection *= -1;
                Rb.velocity = new Vector2(0, Rb.velocity.y);
            }

            if (PinkStar.IsPlayerDetected())
            {
                PinkStar.SwitchState(PinkStar.ChargeState);
            }
        }
    }
}