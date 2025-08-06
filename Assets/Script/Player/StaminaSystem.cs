using System;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem
{
    private class RegenBonus
    {
        public Guid Id;
        public float Rate;
        public float ExpireTime; // Time.time + duration; PositiveInfinity for permanent
    }

    private float currentStamina;
    private float maxStamina;

    private readonly float baseRegenRate;
    private float additionalRegenRate;
    private readonly List<RegenBonus> activeRegenBonuses = new();

    public event Action<float, float> OnStaminaChanged;
    public event Action OnStaminaUsed;
    public event Action OnStaminaFull;

    public StaminaSystem(float initialStamina, float baseRegen)
    {
        maxStamina = initialStamina;
        currentStamina = maxStamina;
        baseRegenRate = baseRegen;
    }

    public void Regenerate()
    {
        TickIfNeeded();

        float totalRegen = GetTotalRegenRate();
        if (currentStamina < maxStamina && totalRegen > 0f)
        {
            Recover(totalRegen * Time.deltaTime);
        }
    }

    public void Use(float amount)
    {
        if (amount <= 0f || currentStamina < amount) return;

        currentStamina = Mathf.Max(currentStamina - amount, 0f);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnStaminaUsed?.Invoke();
    }

    public void Recover(float amount)
    {
        if (amount <= 0f) return;

        float oldStamina = currentStamina;
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        if (oldStamina < maxStamina && currentStamina >= maxStamina)
            OnStaminaFull?.Invoke();
    }

    public Guid ApplyBonusRegen(float rate, float duration = 0f)
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
        bool removed = activeRegenBonuses.RemoveAll(b => b.Id == id) > 0;
        return removed;
    }

    public void RemoveAllTemporaryRegenBuffs()
    {
        activeRegenBonuses.RemoveAll(b => !float.IsPositiveInfinity(b.ExpireTime));
    }

    public void ClearAllRegenBuffs()
    {
        activeRegenBonuses.Clear();
    }

    public void AddPermanentRegen(float amount)
    {
        additionalRegenRate += amount;
    }

    public void IncreaseMaxStamina(float amount, bool refill = false)
    {
        maxStamina += amount;
        if (refill)
            currentStamina = maxStamina;
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
    }

    public float GetCurrentStamina() => currentStamina;
    public float GetMaxStamina() => maxStamina;
    public bool CanUse(float amount) => currentStamina >= amount;

    public float GetTotalRegenRate()
    {
        TickIfNeeded();

        float total = baseRegenRate + additionalRegenRate;
        foreach (var b in activeRegenBonuses)
            total += b.Rate;
        return total;
    }

    // --- internal ticking to expire temporary buffs ---
    public void Tick()
    {
        float now = Time.time;
        int removed = activeRegenBonuses.RemoveAll(b => now >= b.ExpireTime);
        if (removed > 0)
        {
            // optional: notify listeners if you want when total regen changed
            OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }

    private float lastTickTime = -1f;
    private void TickIfNeeded()
    {
        if (!Mathf.Approximately(lastTickTime, Time.time))
        {
            Tick();
            lastTickTime = Time.time;
        }
    }
}
