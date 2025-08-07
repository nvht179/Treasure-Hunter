using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Sword")]
public class Sword : PassiveItemSO
{
    private Guid damageBonusId;
    private Guid criticalBonusId;

    public float damageAmount = 1;
    public float criticalChance = 0.05f;

    private void OnValidate()
    {
        description = "Passive: Provides " + damageAmount.ToString() + " damage, and " + (criticalChance * 100).ToString() + "% critical chance";
        droppable = true;
        consumable = true;
    }
    public override void ApplyEffect(Player player)
    {
        damageBonusId = player.DamageSystem.AddAdditiveDamageBuff(damageAmount);
        criticalBonusId = player.DamageSystem.AddCritChanceBuff(criticalChance);
    }

    public override void RemoveEffect(Player player)
    {
        player.DamageSystem.RemoveBuff(damageBonusId);
        player.DamageSystem.RemoveBuff(criticalBonusId);
    }
}