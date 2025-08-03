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
                var offenderInfo = new IDamageable.DamageInfo
                {
                    Damage = PinkStar.AttackDamage,
                    Force = PinkStar.KnockbackForce,
                    Velocity = Rb.velocity,
                    // KnockbackTime as default
                };
                other.gameObject.GetComponent<Player>().TakeDamage(offenderInfo);
            }

            Rb.velocity = new Vector2(0, Rb.velocity.y);
            PinkStar.SwitchState(PinkStar.RechargeState);
        }

        public override void TakeDamage(IDamageable.DamageInfo offender)
        {
            base.TakeDamage(offender);
            PinkStar.SwitchState(PinkStar.HitState);
        }
    }
}