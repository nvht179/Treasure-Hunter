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
            Rb.velocity = new Vector2(PinkStar.MoveSpeed * PinkStar.MoveDirection, Rb.velocity.y);
            
            if (PinkStar.IsGroundAhead() || PinkStar.IsWallAhead())
            {
                PinkStar.MoveDirection *= -1;
            }

            var playerDetectedRight = PinkStar.CastVisionRay(Vector2.right);
            var playerDetectedLeft = PinkStar.CastVisionRay(Vector2.left);

            if (playerDetectedLeft || playerDetectedRight)
            {
                PinkStar.MoveDirection = playerDetectedLeft ? -1 : 1; 
                PinkStar.SwitchState(PinkStar.ChargeState);
            }
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            PinkStar.SwitchState(PinkStar.HitState);
        }
    }
}