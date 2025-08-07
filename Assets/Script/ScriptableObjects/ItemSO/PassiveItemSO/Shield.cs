using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Shield")]
public class Shield : PassiveItemSO
{
    private Guid maxHealthBuffId;
    private Guid damageReductionBuffId;
    public float maxHealthAdded = 10;
    public float damageReduction = 0.1f; // 10% damage reduction
    private void OnValidate()
    {
        description = "Passive: Provides " + maxHealthAdded.ToString() + " max health and receive " + (damageReduction*100).ToString() + "% damage reduction.";
        droppable = true;
        consumable = true;
    }
    public override void ApplyEffect(Player player)
    {
        maxHealthBuffId = player.HealthSystem.AddMaxHealthBuff(maxHealthAdded);
        damageReductionBuffId = player.DamageReceivedSystem.AddDamageReceivedBuff(1 - damageReduction);
    }

    public override void RemoveEffect(Player player)
    {
        player.HealthSystem.RemoveMaxHealthBuff(maxHealthBuffId);
        player.DamageReceivedSystem.RemoveDamageReceivedBuff(damageReductionBuffId);
    }
}