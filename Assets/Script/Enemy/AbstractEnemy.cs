using UnityEngine;

namespace Script.Enemy
{
    public abstract class AbstractEnemy : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float maxHealth;

        private float currentHealth;
        public float MaxHealth => maxHealth;

        public float CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }

        public Player Player
        {
            get => player;
            set => player = value;
        }

        public void SelfDestroy()
        {
            Destroy(gameObject);
        }
    }
}