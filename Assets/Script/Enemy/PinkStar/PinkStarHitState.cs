using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarHitState : PinkStarBaseState
    {
        private float hitRecoverTimer;

        public PinkStarHitState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStar.CurrentHealth == 0
                ? PinkStarStateManager.PinkStarState.DeadHit
                : PinkStarStateManager.PinkStarState.Hit;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
            hitRecoverTimer = PinkStarStateManager.HitRecoverTime;
        }

        public override void UpdateState()
        {
            hitRecoverTimer -= Time.deltaTime;

            if (hitRecoverTimer < 0)
            {
                PinkStar.SwitchState(PinkStar.WanderState);
            }
        }
    }
}