using UnityEngine;

public class MoveSpeedSystem
{
    private class SpeedBuff
    {
        public float Additional;
        public float ExpireTime;
    }

    private SpeedBuff activeBuff;

    private float baseSpeed;
    private float additiveBonus = 0f;

    public MoveSpeedSystem(float initialBaseSpeed)
    {
        baseSpeed = initialBaseSpeed;
    }

    public void SetBaseSpeed(float speed)
    {
        baseSpeed = Mathf.Max(0, speed);
    }

    public void AddPermanentSpeedBonus(float amount)
    {
        additiveBonus += amount;
    }

    public void AddTemporarySpeed(float addition, float duration)
    {
        activeBuff = new SpeedBuff
        {
            Additional = addition,
            ExpireTime = Time.time + duration
        };
    }

    public float GetCurrentSpeed()
    {
        if (activeBuff != null && Time.time >= activeBuff.ExpireTime)
        {
            activeBuff = null;
        }

        float buffAmount = activeBuff != null ? activeBuff.Additional : 0f;
        float totalMultiplier = 1f + buffAmount;

        return (baseSpeed + additiveBonus) * totalMultiplier;
    }

    public float GetBaseSpeed() => baseSpeed;
}
