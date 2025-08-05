using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Purple Potion SO")]
public class PurplePotionSO : ConsumableItemSO
{
    public float criticalChanceIncrease = 0.2f; // 10% chance to apply critical hit
    public float duration = 20f; // duration of the potion effect in seconds

    private void OnValidate()
    {
        description = "Consumable: Increase " + (criticalChanceIncrease * 100).ToString() + "% critical chance in " + duration + " seconds";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        player.DamageSystem.AddCritChanceBuff(criticalChanceIncrease, duration);
    }
}
