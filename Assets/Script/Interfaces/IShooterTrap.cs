using System;

namespace Script.Interfaces
{
    public interface IShooterTrap
    {
        public event EventHandler<OnShootEventArgs> OnShoot;
        public class OnShootEventArgs : EventArgs
        {
            public IShooterTrap ShooterTrap;
        }
    }
}
