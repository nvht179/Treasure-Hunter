using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarDeadState : PinkStarBaseState
    {
        // state to delay destroying object for playing the DeadGround animation and showing enemy corpse
        private float deadShowTimer;

        public PinkStarDeadState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStarStateManager.PinkStarState.Dead;
            deadShowTimer = PinkStarStateManager.DeadShowTime;
            // TODO: make pink star not collide with the player
        }

        public override void UpdateState()
        {
            deadShowTimer -= Time.deltaTime;
            if (deadShowTimer < 0)
            {
                PinkStar.SelfDestroy();
            }
        }

        public override void TakeDamage(float damage)
        {
            // do nothing
        }
    }
}