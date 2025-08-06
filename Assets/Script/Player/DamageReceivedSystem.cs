using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceivedSystem
{
    private class DamageBuff
    {
        public Guid Id;
        public float Multiplier;   // e.g. 0.8f => take 80% damage, 1.5f => take 150% damage
        public float ExpireTime;   // float.PositiveInfinity for permanent
    }

    private readonly List<DamageBuff> buffs = new();

    private float baseDamageReceived;

    public DamageReceivedSystem(float baseDamageReceived = 1.0f)
    {
        this.baseDamageReceived = Mathf.Max(0f, baseDamageReceived);
    }

    // Buff API
    public Guid AddDamageReceivedBuff(float multiplier, float duration = 0f)
    {
        var b = new DamageBuff
        {
            Id = Guid.NewGuid(),
            Multiplier = Mathf.Max(0f, multiplier),
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        buffs.Add(b);
        return b.Id;
    }
    public bool RemoveDamageReceivedBuff(Guid id)
    {
        bool removed = buffs.RemoveAll(x => x.Id == id) > 0;
        return removed;
    }
    public void RemoveAllTemporaryBuffs()
    {
        int removed = buffs.RemoveAll(x => !float.IsPositiveInfinity(x.ExpireTime));
    }

    public void ClearAllBuffs()
    {
        if (buffs.Count == 0) return;
        buffs.Clear();
    }

    // Base getters / setters
    public void SetBaseDamageReceived(float baseMultiplier)
    {
        baseDamageReceived = Mathf.Max(0f, baseMultiplier);
    }

    public float GetBaseDamageReceived() => baseDamageReceived;

    // Tick / expiration
    public void Tick()
    {
        float now = Time.time;
        int removed = buffs.RemoveAll(b => now >= b.ExpireTime);
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

    // Query / calculation
    public float GetCurrentMultiplier()
    {
        TickIfNeeded();

        float mult = baseDamageReceived;
        foreach (var b in buffs)
            mult *= b.Multiplier;
        return Mathf.Max(0f, mult);
    }

    public float CalculateReceivedDamage(float incomingDamage)
    {
        if (incomingDamage <= 0f) return 0f;
        float m = GetCurrentMultiplier();
        return incomingDamage * m;
    }
}
