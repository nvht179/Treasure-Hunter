using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarRechargeState : PinkStarBaseState
    {
        private float rechargeTimer;

        public PinkStarRechargeState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStarStateManager.PinkStarState.Recharge;
            Rb.velocity = new Vector2(0, Rb.velocity.y);
            rechargeTimer = PinkStar.RechargeTime;
        }

        public override void UpdateState()
        {
            rechargeTimer -= Time.deltaTime;
            if (rechargeTimer < 0)
            {
                PinkStar.SwitchState(PinkStar.WanderState);
            }
        }
        
        public override void TakeDamage(IDamageable.DamageInfo offender)
        {
            base.TakeDamage(offender);
            PinkStar.SwitchState(PinkStar.HitState);
        }
    }
}