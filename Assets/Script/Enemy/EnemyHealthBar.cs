using UnityEngine;
using UnityEngine.UI;

namespace Script.Enemy
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject enemyGameObject;
        [SerializeField] private Image barImage;

        private IDamageable enemy;
        private void Start()
        {
            enemy = enemyGameObject.GetComponent<IDamageable>();
            enemy.OnDamageTaken += EnemyOnDamageTaken;
            barImage.fillAmount = 1;
            gameObject.SetActive(false);
        }

        private void EnemyOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
        {
            var healthNormalized = e.CurrentHealth / e.MaxHealth;
            barImage.fillAmount = healthNormalized;
            if (healthNormalized == 0f || Mathf.Approximately(healthNormalized, 1f))
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Show()
        {
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}