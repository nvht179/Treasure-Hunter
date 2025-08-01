using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarAttackState : PinkStarBaseState
    {
        public PinkStarAttackState(PinkStarStateManager stateManager) : base(stateManager)
        {
        }

        public override void EnterState()
        {
            CurrentState = PinkStarStateManager.PinkStarState.Attack;
            Rb.velocity = new Vector2(PinkStar.ChargeSpeed * PinkStar.MoveDirection, Rb.velocity.y);
        }

        public override void UpdateState()
        {
            Rb.velocity = new Vector2(PinkStar.ChargeSpeed * PinkStar.MoveDirection, Rb.velocity.y);
            if (PinkStar.IsWallAhead())
            {
                PinkStar.SwitchState(PinkStar.RechargeState);
            }
        }

        public override void OnCollisionEnter(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<Player>().TakeDamage(PinkStar.AttackDamage);
            }
            
            PinkStar.SwitchState(PinkStar.RechargeState);
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            PinkStar.SwitchState(PinkStar.HitState);
        }
    }
}