using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Orange Potion SO")]
public class OrangePotionSO : ConsumableItemSO
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
