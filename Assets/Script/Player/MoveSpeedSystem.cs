using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedSystem
{
    private class SpeedBuff
    {
        public Guid Id;
        public float Multiplier;   // e.g. 0.1 means +10% speed
        public float ExpireTime;   // Time.time + duration; PositiveInfinity = permanent
        public bool IsPermanent => float.IsPositiveInfinity(ExpireTime);
    }

    private readonly List<SpeedBuff> buffs = new();
    private float baseSpeed;

    public MoveSpeedSystem(float initialBaseSpeed)
    {
        baseSpeed = Mathf.Max(0f, initialBaseSpeed);
    }

    public void SetBaseSpeed(float speed)
    {
        baseSpeed = Mathf.Max(0f, speed);
    }

    public float GetBaseSpeed() => baseSpeed;

    public Guid AddMoveSpeedMultiplier(float multiplier, float duration = 0f)
    {
        var buff = new SpeedBuff
        {
            Id = Guid.NewGuid(),
            Multiplier = multiplier,
            ExpireTime = duration > 0f ? Time.time + duration : float.PositiveInfinity
        };
        buffs.Add(buff);
        return buff.Id;
    }

    public bool RemoveBuff(Guid id)
    {
        return buffs.RemoveAll(b => b.Id == id) > 0;
    }

    public void RemoveAllTemporaryBuffs()
    {
        buffs.RemoveAll(b => !b.IsPermanent);
    }

    public void ClearAllBuffs()
    {
        buffs.Clear();
    }

    public float GetCurrentSpeed()
    {
        TickIfNeeded();

        float totalMultiplier = 1f;
        foreach (var buff in buffs)
        {
            totalMultiplier *= (1f + buff.Multiplier);
        }

        return baseSpeed * totalMultiplier;
    }

    // Expire old buffs once per frame
    public void Tick()
    {
        float now = Time.time;
        buffs.RemoveAll(b => !b.IsPermanent && now >= b.ExpireTime);
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
