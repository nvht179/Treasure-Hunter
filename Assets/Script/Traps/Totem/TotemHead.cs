using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHead : ShooterTrap, IDamageable {

    public event EventHandler<IDamageable.OnDamageTakenEventArgs> OnDamageTaken;
    public event EventHandler OnDestroyed;

    [SerializeField] private float maxHealthPoint = 20f;

    private float currentHealthPoint;
    private bool isDead = false;

    private new void Awake()
    {
        base.Awake();
        fireDelay = 0.15f;
        currentHealthPoint = maxHealthPoint;
    }

    public void TakeDamage(IDamageable.DamageInfo offenderInfo) {
        currentHealthPoint = Mathf.Clamp(currentHealthPoint - offenderInfo.Damage, 0, maxHealthPoint);
        OnDamageTaken?.Invoke(this, new IDamageable.OnDamageTakenEventArgs {
            MaxHealth = maxHealthPoint,
            CurrentHealth = currentHealthPoint
        });
        if (currentHealthPoint <= 0 && !isDead) {
            isDead = true;
            OnDestroyed?.Invoke(this, EventArgs.Empty);
            StopShooting();
            if (TryGetComponent<Collider2D>(out var col2d)) {
                col2d.enabled = false;
            }
            CoinSpawner.Instance.SpawnCoin(transform.position);
            Destroy(gameObject, 2f);
        }
    }
}
