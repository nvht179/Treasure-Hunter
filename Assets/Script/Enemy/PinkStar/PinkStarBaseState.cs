using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public abstract class PinkStarBaseState
    {
        protected readonly PinkStarStateManager PinkStar;
        protected readonly Rigidbody2D Rb;
        protected PinkStarStateManager.PinkStarState CurrentState;

        protected PinkStarBaseState(PinkStarStateManager stateManager)
        {
            PinkStar = stateManager;
            Rb = PinkStar.Rb;
        }

        public PinkStarStateManager.PinkStarState GetCurrentState()
        {
            return CurrentState;
        }
        
        public abstract void EnterState();
        public abstract void UpdateState();

        public virtual void OnCollisionEnter(Collision2D other)
        {
        }

        public virtual void TakeDamage(float damage)
        {
            PinkStar.CurrentHealth -= damage;
        }
    }
}