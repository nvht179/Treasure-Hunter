using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seashell : ShooterTrap, IDamageable {

    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;
    public event EventHandler OnDestroyed;

    [SerializeField] private ShooterTrapSO seaShellSO;
    [SerializeField] private int maxHealth;
    [SerializeField] private int minMoneyOnDead;
    [SerializeField] private int maxMoneyOnDead;
    private float currentHealthPoint;
    private bool isDead;

    private new void Awake() {
        base.Awake();
        FireDelay = 0.15f;
        currentHealthPoint = maxHealth;
    }

    public void TakeDamage(IDamageable.DamageInfo offenderInfo) {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - offenderInfo.Damage, 0, maxHealth);
            
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs {
            MaxHealth = seaShellSO.maxHealth,
            CurrentHealth = currentHealthPoint
        });
        if (currentHealthPoint <= 0 && !isDead)
        {
            isDead = true;
            OnDestroyed?.Invoke(this, EventArgs.Empty);
            StopShooting();
            ResourceSpawner.Instance.SpawnMoney(transform.position, minMoneyOnDead, maxMoneyOnDead);
            Destroy(gameObject, 2f);

            GameManager.Instance.AddScoreOnEnemyDead(seaShellSO.scoreOnDead);
        }
    }
}
