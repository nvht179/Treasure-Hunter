using System;
using UnityEngine;

namespace Script.Enemy
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private ScriptableObjects.EnemySO enemySO;

        public event EventHandler<OnDestroyedEventArgs> OnDestroyed;

        public class OnDestroyedEventArgs : EventArgs
        {
            public AbstractEnemy Enemy;
        }

        private float currentHealth;
        public float MaxHealth => enemySO.maxHealth;

        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Clamp(value, 0, enemySO.maxHealth);
        }

        public Player Player
        {
            get => player;
            set => player = value;
        }

        public virtual void SelfDestroy()
        {
            Destroy(gameObject);
            OnDestroyed?.Invoke(this, new OnDestroyedEventArgs
            {
                Enemy = this
            });

            GameManager.Instance.AddScoreOnEnemyDead(enemySO.scoreOnDead);
        }
        public virtual void TakeDamage(IDamageable.DamageInfo offenderInfo)
        {
        }
    }
}