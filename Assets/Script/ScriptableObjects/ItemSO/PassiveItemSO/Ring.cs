using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Ring")]
public class Ring : PassiveItemSO
{
    private Guid id;

    public float bonusGoldMultiplier = 0.1f; // e.g., 0.1 means +10% gold

    private void OnValidate()
    {
        description = "Passive: Receive " + (bonusGoldMultiplier*100).ToString() + "% more gold";
        droppable = true;
        consumable = false;
    }
    public override void ApplyEffect(Player player)
    {
        id = player.GoldBonusSystem.AddGoldBonus(bonusGoldMultiplier);
    }

    public override void RemoveEffect(Player player)
    {
        player.GoldBonusSystem.RemoveGoldBonus(id);
    }
}
