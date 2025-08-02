using UnityEngine;

namespace Script.Enemy.BossStar
{
    public abstract class BossStarBaseStage
    {
        protected readonly BossStarContext BossStar;
        protected readonly Rigidbody2D Rb;
        
        protected BossStarBaseStage(BossStarContext context)
        {
            BossStar = context;
            Rb = BossStar.Rb;
        }
        
        public abstract void EnterState();
        public abstract void UpdateState();

        public virtual void TakeDamage(float damage)
        {
            BossStar.CurrentHealth -= damage;
        }
    }
}