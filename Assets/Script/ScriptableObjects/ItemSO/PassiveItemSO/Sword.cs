using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Sword")]
public class Sword : PassiveItemSO
{
    private Guid id;

    public float damageAmount = 1;
    public float criticalChance = 0.05f;

    private void OnValidate()
    {
        description = "Passive: Provides " + damageAmount.ToString() + " damage, and " + (criticalChance * 100).ToString() + "% critical chance";
        droppable = true;
        consumable = false;
    }
    public override void ApplyEffect(Player player)
    {
        id = player.DamageSystem.AddAdditiveDamageBuff(damageAmount);
    }

    public override void RemoveEffect(Player player)
    {
        player.DamageSystem.RemoveBuff(id);
    }
}