using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public struct DamageInfo
    {
        // knockback velocity = velocity * force
        // velocity here means velocity of the offender
        // knockback velocity will remain for knockbackTIme
        public float Damage;
        public float Force;
        public Vector2 Velocity; 
        private float knockbackTime;
        public const float DefaultKnockbackTime = 0.5f;
        public float KnockbackTime
        {
            get => knockbackTime == 0f ? DefaultKnockbackTime : knockbackTime;
            set => knockbackTime = value;
        }
    }

    public event EventHandler OnDestroyed;
    public event EventHandler<OnDamageTakenEventArgs> OnDamageTaken;

    public class OnDamageTakenEventArgs
    {
        public float MaxHealth;
        public float CurrentHealth;
    }

    void TakeDamage(DamageInfo offenderInfo);
}