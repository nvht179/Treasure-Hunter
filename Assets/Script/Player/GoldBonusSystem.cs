using System;
using System.Collections.Generic;
using UnityEngine;

public class GoldBonusSystem
{
    private class GoldBonus
    {
        public Guid Id;
        public float Multiplier; // e.g., 0.5f means +50% gold
        public float ExpireTime; // PositiveInfinity for permanent
    }

    private readonly List<GoldBonus> activeGoldBonuses = new();

    private float baseGoldBonus;

    public GoldBonusSystem(float baseGoldBonus = 1.0f)
    {
        this.baseGoldBonus = baseGoldBonus;
    }

    // --- Add Buff ---
    public Guid AddGoldBonus(float multiplier, float duration = 0f)
    {
        var bonus = new GoldBonus
        {
            Id = Guid.NewGuid(),
            Multiplier = multiplier,
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        activeGoldBonuses.Add(bonus);
        return bonus.Id;
    }

    // --- Remove Buff ---
    public bool RemoveGoldBonus(Guid id)
    {
        return activeGoldBonuses.RemoveAll(b => b.Id == id) > 0;
    }

    // --- Calculate Current Bonus ---
    public float GetCurrentGoldMultiplier()
    {
        float now = Time.time;
        activeGoldBonuses.RemoveAll(b => now >= b.ExpireTime);

        float totalMultiplier = baseGoldBonus;
        foreach (var bonus in activeGoldBonuses)
            totalMultiplier += bonus.Multiplier; // additive stacking

        return totalMultiplier;
    }

    // --- Apply Bonus to Gold Amount ---
    public int ApplyGoldBonus(int baseGoldAmount)
    {
        return Mathf.RoundToInt(baseGoldAmount * GetCurrentGoldMultiplier());
    }

    // --- Set Base Bonus ---
    public void SetBaseGoldBonus(float multiplier)
    {
        baseGoldBonus = Mathf.Max(0f, multiplier);
    }
}
