using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Shield")]
public class Shield : PassiveItemSO
{
    private Guid id;
    public float maxHealthAdded = 10;
    private void OnValidate()
    {
        description = "Passive: Provides " + maxHealthAdded.ToString() + " max health.";
        droppable = true;
        consumable = false;
    }
    public override void ApplyEffect(Player player)
    {
        id = player.HealthSystem.AddMaxHealthBuff(maxHealthAdded);
    }

    public override void RemoveEffect(Player player)
    {
        player.HealthSystem.RemoveMaxHealthBuff(id);
    }
}