using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem
{
    private class RegenBonus
    {
        public float Rate;
        public float ExpireTime;
    }

    private float currentHealth;
    private float maxHealth;

    private readonly float baseRegenRate;
    private float additionalRegenRate;
    private readonly List<RegenBonus> activeRegenBonuses = new();

    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;

    public class OnHealthChangedEventArgs
    {
        public float CurrentHealth;
        public float MaxHealth;
    }
    public event EventHandler OnDamageReceived;
    public event Action OnDeath;

    public HealthSystem(float initialHealth, float baseRegen)
    {
        maxHealth = initialHealth;
        currentHealth = maxHealth;
        baseRegenRate = baseRegen;
    }

    public void Regenerate()
    {
        activeRegenBonuses.RemoveAll(b => Time.time >= b.ExpireTime);

        float totalBonus = 0f;
        foreach (var b in activeRegenBonuses)
            totalBonus += b.Rate;

        float totalRegen = baseRegenRate + additionalRegenRate + totalBonus;
        if (currentHealth < maxHealth && totalRegen > 0)
        {
            Heal(totalRegen * Time.deltaTime);
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            CurrentHealth = currentHealth,
            MaxHealth = maxHealth
        });
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            CurrentHealth = currentHealth,
            MaxHealth = maxHealth
        });
        OnDamageReceived?.Invoke(this, EventArgs.Empty);
        if (currentHealth <= 0)
            OnDeath?.Invoke();
    }

    public void LosePercentCurrentHealth(float percent)
    {
        if (percent <= 0 || percent > 1) return;
        float damage = currentHealth * percent;
        TakeDamage(damage);
    }
    public void ApplyBonusRegen(float rate, float duration)
    {
        activeRegenBonuses.Add(new RegenBonus
        {
            Rate = rate,
            ExpireTime = Time.time + duration
        });
    }

    public void AddPermanentRegen(float amount)
    {
        additionalRegenRate += amount;
    }

    public void IncreaseMaxHealth(float amount, bool healToFull = false)
    {
        maxHealth += amount;
        if (healToFull)
            currentHealth = maxHealth;
        
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            CurrentHealth = currentHealth,
            MaxHealth = maxHealth
        });
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
