using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Purple Potion SO")]
public class PurplePotionSO : ConsumableItemSO
{

    private void OnValidate()
    {
        description = "";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {

    }
}
