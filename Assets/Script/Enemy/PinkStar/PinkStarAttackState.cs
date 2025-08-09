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
            if (PinkStar.IsWallAhead() || !PinkStar.IsGroundAhead())
            {
                PinkStar.SwitchState(PinkStar.RechargeState);
            }

            HandleAttack();
        }

        public void HandleAttack()
        {
            var enemiesInRange = new Collider2D[10];
            _ = Physics2D.OverlapCircleNonAlloc(PinkStar.Pivot.position, PinkStarStateManager.AttackRadius,
                enemiesInRange, PinkStar.PlayerLayer);
            foreach (var enemy in enemiesInRange)
            {
                // make enemy take damage
                if (enemy != null)
                {
                    var player = enemy.GetComponent<Player>();
                    var offenderInfo = new IDamageable.DamageInfo
                    {
                        Damage = PinkStar.AttackDamage,
                        Force = PinkStar.KnockbackForce,
                        Velocity = Rb.velocity,
                        // KnockbackTime as default
                    };
                    if (player != null)
                    {
                        player.TakeDamage(offenderInfo);
                        PinkStar.TriggerAttack(); // Trigger the attack sound event
                        PinkStar.SwitchState(PinkStar.RechargeState);
                    }
                }
            }
        }

        public override void TakeDamage(IDamageable.DamageInfo offender)
        {
            base.TakeDamage(offender);
            PinkStar.SwitchState(PinkStar.HitState);
        }
    }
}