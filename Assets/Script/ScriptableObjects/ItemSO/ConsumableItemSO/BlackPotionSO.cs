using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Black Potion SO")]
public class BlackPotionSO : ConsumableItemSO
{
    public float goldMultiplier = 1.5f;
    public float duration = 60f;

    private void OnValidate()
    {
        description = "Consumable: Earn more " + goldMultiplier.ToString() + " gold multiplier in " + duration.ToString() + " seconds but you will receive double damage.";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        player.ApplyBlackPotionEffect(goldMultiplier, duration);
    }
}
