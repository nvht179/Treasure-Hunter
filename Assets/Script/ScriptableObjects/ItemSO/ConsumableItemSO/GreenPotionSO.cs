using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Green Potion SO")]
public class GreenPotionSO : ConsumableItemSO
{
    private void OnValidate()
    {
        description = "Consumable: let's go gambling. lose half of your current health or restore full health";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        if (Random.value < 0.5f)
        {
            player.HealthSystem.LosePercentCurrentHealth(0.5f);
        }
        else
        {
            player.HealthSystem.Heal(player.HealthSystem.GetMaxHealth());
        }
    }
}