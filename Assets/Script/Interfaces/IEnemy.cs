using System;

namespace Script.Interfaces
{
    public interface IEnemy
    {
        public event EventHandler<OnAttackEventArgs> OnAttack;

        public class OnAttackEventArgs : EventArgs
        {
            public IEnemy Enemy;
        }
    }
}
