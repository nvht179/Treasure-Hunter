using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Health Potion SO")]
public class HealthPotionSO : ConsumableItemSO
{
    public float healAmount = 10;

    private void OnValidate()
    {
        description = "Consumable: Restores " + healAmount.ToString() + "HP";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player, out float duration)
    {
        duration = 0f;
        player.HealthSystem.Heal(healAmount);
    }
}