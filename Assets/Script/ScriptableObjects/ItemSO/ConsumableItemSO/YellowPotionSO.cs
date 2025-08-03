using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Yellow Potion SO")]
public class YellowPotionSO : ConsumableItemSO
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
