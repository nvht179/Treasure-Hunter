using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageBuffSystem
{
    private class DamageBuff
    {
        public float Multiplier;
        public float ExpireTime;
    }

    private class CritChanceBuff
    {
        public float BonusChance;
        public float ExpireTime;
    }

    private readonly List<DamageBuff> activeDamageBuffs = new();
    private readonly List<CritChanceBuff> activeCritBuffs = new();

    private float baseDamageMultiplier = 1f;

    private float baseCritChance = 0f;     // e.g., 0.15 = 15%
    private float critMultiplier = 2f;     // e.g., 2x damage

    private readonly System.Random rng = new();

    public PlayerDamageBuffSystem(float initialCritChance = 0f, float initialCritMultiplier = 2f)
    {
        baseCritChance = initialCritChance;
        critMultiplier = initialCritMultiplier;
    }

    public void AddTemporaryDamageBuff(float multiplier, float duration)
    {
        activeDamageBuffs.Add(new DamageBuff
        {
            Multiplier = multiplier,
            ExpireTime = Time.time + duration
        });
    }

    public void AddTemporaryCritChance(float bonusChance, float duration)
    {
        activeCritBuffs.Add(new CritChanceBuff
        {
            BonusChance = bonusChance,
            ExpireTime = Time.time + duration
        });
    }

    public void AddPermanentDamageMultiplier(float multiplier)
    {
        baseDamageMultiplier *= multiplier;
    }

    public void SetCritChance(float chance)
    {
        baseCritChance = Mathf.Clamp01(chance);
    }

    public void AddCritMultiplier(float multiplier)
    {
        critMultiplier *= multiplier;
    }

    public float CalculateOutgoingDamage(float rawDamage, out bool isCrit)
    {
        float time = Time.time;
        activeDamageBuffs.RemoveAll(buff => time >= buff.ExpireTime);
        activeCritBuffs.RemoveAll(buff => time >= buff.ExpireTime);

        // Total damage multiplier
        float totalMultiplier = baseDamageMultiplier;
        foreach (var buff in activeDamageBuffs)
            totalMultiplier *= buff.Multiplier;

        // Total crit chance
        float totalCritChance = baseCritChance;
        foreach (var critBuff in activeCritBuffs)
            totalCritChance += critBuff.BonusChance;
        totalCritChance = Mathf.Clamp01(totalCritChance);

        isCrit = rng.NextDouble() < totalCritChance;
        if (isCrit)
            totalMultiplier *= critMultiplier;

        float finalDamage = rawDamage * totalMultiplier;

        return finalDamage;
    }

    public float GetCritChance() => baseCritChance;
    public float GetCritMultiplier() => critMultiplier;
    public float GetBaseMultiplier() => baseDamageMultiplier;
}
