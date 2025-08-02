using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarHitState : PinkStarBaseState
    {
        private bool isDeadHit;

        private float hitRecoverTimer;

        public PinkStarHitState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStar.CurrentHealth <= 0
                ? PinkStarStateManager.PinkStarState.DeadHit
                : PinkStarStateManager.PinkStarState.Hit;
            isDeadHit = PinkStar.CurrentHealth <= 0;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
            hitRecoverTimer = PinkStarStateManager.HitRecoverTime;
        }

        public override void UpdateState()
        {
            hitRecoverTimer -= Time.deltaTime;

            if (hitRecoverTimer < 0)
            {
                if (isDeadHit)
                {
                    PinkStar.SwitchState(PinkStar.DeadState);
                }
                else
                {
                    PinkStar.SwitchState(PinkStar.WanderState);
                }
            }
        }

        public override void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
            if (!isDeadHit)
            {
                base.TakeDamage(offenderInfo);
                // if pink star take damage during hit state, re-enter the hit state
                PinkStar.SwitchState(PinkStar.HitState);
            }
        }
    }
}