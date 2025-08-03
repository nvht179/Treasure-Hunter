using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Green Potion SO")]
public class GreenPotionSO : ConsumableItemSO
{
    private void OnValidate()
    {
        description = "consumable: let's go gambling. lose half of your current health or restore full health";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        if (Random.value < 0.5f)
        {
            player.GetHealthSystem().LosePercentCurrentHealth(0.5f);
        }
        else
        {
            player.GetHealthSystem().Heal(player.GetHealthSystem().GetMaxHealth());
        }
    }
}