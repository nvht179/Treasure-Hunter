using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Orange Potion SO")]
public class OrangePotionSO : ConsumableItemSO
{
    public float criticalDamageMultiplier = 1.1f;

    private void OnValidate()
    {
        description = "Increase your critical damage by " + criticalDamageMultiplier + "% permanently";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        player.DamageSystem.AddCritMultiplierBuff(criticalDamageMultiplier);
    }
}
