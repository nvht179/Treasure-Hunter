using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem
{
    private enum BuffKind { AdditiveDamage, CritChance, CritMultiplier }

    private class Buff
    {
        public Guid Id;
        public BuffKind Kind;
        public float Value;       // bonus chance for crit / multiplier for crit multiplier
        public float ExpireTime;  // if duration <= 0 => permanent => ExpireTime = float.PositiveInfinity
    }

    private readonly List<Buff> buffs = new();

    private float baseAttackDamage;
    private float baseCritChance;    // 0..1
    private float baseCritMultiplier; // >= 1 (e.g. 2 = 200% damage on crit)


    public DamageSystem(float baseAttackDamage, float baseCritChance, float baseCritMultiplier)
    {
        this.baseAttackDamage = Mathf.Max(0f, baseAttackDamage);
        this.baseCritChance = Mathf.Clamp01(baseCritChance);
        this.baseCritMultiplier = Mathf.Max(1f, baseCritMultiplier);
    }

    // --- Public API for buffs ---
    public Guid AddAdditiveDamageBuff(float amount, float duration = 0f)
    {
        var buff = new Buff
        {
            Id = Guid.NewGuid(),
            Kind = BuffKind.AdditiveDamage,
            Value = amount,
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        buffs.Add(buff);
        return buff.Id;
    }

    public Guid AddCritChanceBuff(float bonusChance, float duration = 0f)
    {
        var buff = new Buff
        {
            Id = Guid.NewGuid(),
            Kind = BuffKind.CritChance,
            Value = bonusChance,
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        buffs.Add(buff);
        return buff.Id;
    }

    public Guid AddCritMultiplierBuff(float multiplier, float duration = 0f)
    {
        var buff = new Buff
        {
            Id = Guid.NewGuid(),
            Kind = BuffKind.CritMultiplier,
            Value = multiplier,
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        buffs.Add(buff);
        return buff.Id;
    }

    public bool RemoveBuff(Guid id)
    {
        var removed = buffs.RemoveAll(b => b.Id == id) > 0;
        return removed;
    }

    public void RemoveAllTemporaryBuffs()
    {
        int removed = buffs.RemoveAll(b => !float.IsPositiveInfinity(b.ExpireTime));
    }

    public void ClearAllBuffs()
    {
        buffs.Clear();
    }

    // --- Base setters/getters ---

    public void SetBaseAttackDamage(float value)
    {
        baseAttackDamage = Mathf.Max(0f, value);
    }

    public float GetBaseAttackDamage() => baseAttackDamage;

    public void SetBaseCritChance(float chance)
    {
        baseCritChance = Mathf.Clamp01(chance);
    }

    public float GetBaseCritChance() => baseCritChance;

    public void SetBaseCritMultiplier(float multiplier)
    {
        baseCritMultiplier = Mathf.Max(1f, multiplier);
    }

    public float GetBaseCritMultiplier() => baseCritMultiplier;

    // --- Query / Calculate ---

    // Call this periodically to clean up expired buffs
    public void Tick()
    {
        float now = Time.time;
        int removed = buffs.RemoveAll(b => now >= b.ExpireTime);
    }

    public float GetTotalAdditiveDamage()
    {
        TickIfNeeded(); // convenience: ensure expired buffs cleaned
        float sum = 0f;
        foreach (var b in buffs)
            if (b.Kind == BuffKind.AdditiveDamage) sum += b.Value;
        return sum;
    }
    public float GetEffectiveAttackDamage()
    {
        return baseAttackDamage + GetTotalAdditiveDamage();
    }

    public float GetCurrentCritChance()
    {
        TickIfNeeded();
        float total = baseCritChance;
        foreach (var b in buffs)
            if (b.Kind == BuffKind.CritChance) total += b.Value;
        return Mathf.Clamp01(total);
    }

    public float GetCurrentCritMultiplier()
    {
        TickIfNeeded();
        float mult = baseCritMultiplier;
        // we treat crit-mult buffs multiplicatively so e.g. two 1.2 buffs => x1.44
        foreach (var b in buffs)
            if (b.Kind == BuffKind.CritMultiplier) mult *= b.Value;
        return Mathf.Max(1f, mult);
    }

    public float CalculateOutgoingDamage(float rawDamage, out bool isCritical)
    {
        TickIfNeeded();

        float effectiveBase = GetEffectiveAttackDamage();
        // We apply additive buffs to base first (if you prefer to apply to rawDamage instead, change order).
        float preMultDamage = rawDamage + effectiveBase;

        // Determine crit
        float critChance = GetCurrentCritChance();
        isCritical = UnityEngine.Random.value < critChance;

        float critMult = isCritical ? GetCurrentCritMultiplier() : 1f;
        float final = preMultDamage * critMult;

        return final;
    }

    // Convenience: calculate using the system's effectiveBase as raw
    public float CalculateOutgoingDamageUsingBase(out bool isCritical)
    {
        return CalculateOutgoingDamage(0f, out isCritical);
    }

    // --- Internals ---

    private float lastTickTime = -1f;
    private void TickIfNeeded()
    {
        // Avoid calling RemoveAll each time if Time.time hasn't advanced (micro-optimization).
        if (Mathf.Approximately(lastTickTime, Time.time) == false)
        {
            Tick();
            lastTickTime = Time.time;
        }
    }
}
