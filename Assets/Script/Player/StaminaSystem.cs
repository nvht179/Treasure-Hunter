using System.Collections.Generic;
using UnityEngine;
using System;

public class StaminaSystem
{
    private class RegenBonus
    {
        public float rate;
        public float expireTime;
    }

    private float currentStamina;
    private float maxStamina;

    private float baseRegenRate;
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
        activeRegenBonuses.RemoveAll(b => Time.time >= b.expireTime);

        float totalBonus = 0f;
        foreach (var b in activeRegenBonuses)
            totalBonus += b.rate;

        float totalRegen = baseRegenRate + additionalRegenRate + totalBonus;
        if (currentStamina < maxStamina && totalRegen > 0)
        {
            Recover(totalRegen * Time.deltaTime);
        }
    }

    public void Use(float amount)
    {
        if (amount <= 0 || currentStamina < amount) return;

        currentStamina = Mathf.Max(currentStamina - amount, 0);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);
        OnStaminaUsed?.Invoke();
    }

    public void Recover(float amount)
    {
        if (amount <= 0) return;

        float oldStamina = currentStamina;
        currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        if (oldStamina < maxStamina && currentStamina >= maxStamina)
        {
            OnStaminaFull?.Invoke();
        }
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
}
