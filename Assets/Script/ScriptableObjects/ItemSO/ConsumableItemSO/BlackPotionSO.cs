using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Black Potion SO")]
public class BlackPotionSO : ConsumableItemSO
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
