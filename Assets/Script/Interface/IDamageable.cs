using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

    public event EventHandler OnDestroyed;
    public event EventHandler<OnDamageTakenEventArgs> OnDamageTaken;

    public class OnDamageTakenEventArgs {
        public float MaxHealth;
        public float CurrentHealth;
    }
    void TakeDamage(float damage);
}
