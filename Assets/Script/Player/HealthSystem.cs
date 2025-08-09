using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    private class RegenBonus
    {
        public Guid Id;
        public float Rate;
        public float ExpireTime; // PositiveInfinity for permanent
    }

    private class MaxHealthBonus
    {
        public Guid Id;
        public float Amount; // Amount added to base max health
    }

    // --- Fields ---
    private readonly List<RegenBonus> activeRegenBonuses = new();
    private readonly List<MaxHealthBonus> activeMaxHealthBonuses = new();

    private float currentHealth;
    private float baseMaxHealth;
    private readonly float baseRegenRate;
    private float additionalRegenRate;

    // --- Events ---
    public event EventHandler<OnHealthChangedEventArgs> OnHealthChanged;
    public class OnHealthChangedEventArgs
    {
        public float CurrentHealth;
        public float MaxHealth;
    }
    public event EventHandler OnDamageReceived;
    public event EventHandler OnDeath;

    // --- Constructor ---
    public HealthSystem(float initialHealth, float baseRegen)
    {
        baseMaxHealth = initialHealth;
        currentHealth = baseMaxHealth;
        baseRegenRate = baseRegen;
    }

    // --- Public API ---
    public void Tick()
    {
        float now = Time.time;
        activeRegenBonuses.RemoveAll(b => now >= b.ExpireTime);
    }

    public void Regenerate()
    {
        Tick();
        float totalRegen = GetTotalRegenRate();
        if (totalRegen > 0f && currentHealth < GetMaxHealth())
        {
            Heal(totalRegen * Time.deltaTime);
        }
    }

    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Min(currentHealth + amount, GetMaxHealth());
        RaiseHealthChanged();
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, GetMaxHealth());
        RaiseHealthChanged();
        OnDamageReceived?.Invoke(this, EventArgs.Empty);
        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            Debug.Log("HealthSystem: Player has died.");
        }
        else
            Debug.Log($"HealthSystem: Took damage, current health is now {currentHealth}");
    }

    public void Sink()
    {
        currentHealth = 0f;
        RaiseHealthChanged();
        OnDeath?.Invoke(this, EventArgs.Empty);
    }

    public void LosePercentCurrentHealth(float percent)
    {
        if (percent <= 0f || percent > 1f) return;
        TakeDamage(currentHealth * percent);
    }

    // --- Regen Buffs ---
    public Guid AddRegenBuff(float rate, float duration = 0f)
    {
        var buff = new RegenBonus
        {
            Id = Guid.NewGuid(),
            Rate = rate,
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        activeRegenBonuses.Add(buff);
        return buff.Id;
    }

    public bool RemoveRegenBuff(Guid id)
    {
        return activeRegenBonuses.RemoveAll(b => b.Id == id) > 0;
    }

    public void AddPermanentRegen(float amount)
    {
        additionalRegenRate += amount;
    }

    public float GetTotalRegenRate()
    {
        float total = baseRegenRate + additionalRegenRate;
        foreach (var b in activeRegenBonuses)
            total += b.Rate;
        return total;
    }

    // --- Max Health Buffs ---
    public Guid AddMaxHealthBuff(float amount)
    {
        var buff = new MaxHealthBonus
        {
            Id = Guid.NewGuid(),
            Amount = amount
        };
        activeMaxHealthBonuses.Add(buff);
        RaiseHealthChanged();
        Heal(amount);
        return buff.Id;
    }

    public bool RemoveMaxHealthBuff(Guid id)
    {
        bool removed = activeMaxHealthBonuses.RemoveAll(b => b.Id == id) > 0;
        if (removed) RaiseHealthChanged();
        return removed;
    }

    public void IncreaseBaseMaxHealth(float amount, bool healToFull = false)
    {
        baseMaxHealth += amount;
        if (healToFull)
            currentHealth = GetMaxHealth();
        RaiseHealthChanged();
    }

    public float GetMaxHealth()
    {
        float bonus = 0f;
        foreach (var b in activeMaxHealthBonuses)
            bonus += b.Amount;
        return baseMaxHealth + bonus;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetBaseMaxHealth() => baseMaxHealth;
    public float GetBaseRegenRate() => baseRegenRate;

    // --- Internal ---
    private void RaiseHealthChanged()
    {
        OnHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            CurrentHealth = currentHealth,
            MaxHealth = GetMaxHealth()
        });
    }
}
