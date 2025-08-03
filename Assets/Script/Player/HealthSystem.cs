using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthSystem
{
    private class RegenBonus
    {
        public float rate;
        public float expireTime;
    }

    private float currentHealth;
    private float maxHealth;

    private float baseRegenRate;
    private float additionalRegenRate;
    private readonly List<RegenBonus> activeRegenBonuses = new();

    public event Action<float, float> OnHealthChanged;
    public event Action OnReceiveDamage;
    public event Action OnDeath;

    public HealthSystem(float initialHealth, float baseRegen)
    {
        maxHealth = initialHealth;
        currentHealth = maxHealth;
        baseRegenRate = baseRegen;
    }

    public void Regenerate()
    {
        activeRegenBonuses.RemoveAll(b => Time.time >= b.expireTime);

        float totalBonus = 0f;
        foreach (var b in activeRegenBonuses)
            totalBonus += b.rate;

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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnReceiveDamage?.Invoke();
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
            rate = rate,
            expireTime = Time.time + duration
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
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
