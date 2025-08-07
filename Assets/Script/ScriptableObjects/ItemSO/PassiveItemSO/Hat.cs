using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Hat")]
public class Hat : PassiveItemSO
{
    private Guid id;

    public float healthRegenerationAmount = 0.1f; // e.g., 0.1 means +10% health regeneration

    private void OnValidate()
    {
        description = "Passive: Provides " + (healthRegenerationAmount * 100).ToString() + "% health regeneration";
        droppable = true;
        consumable = true;
    }
    public override void ApplyEffect(Player player)
    {
        player.HealthSystem.AddRegenBuff(healthRegenerationAmount);
    }

    public override void RemoveEffect(Player player)
    {
        player.HealthSystem.RemoveRegenBuff(id);
    }
}