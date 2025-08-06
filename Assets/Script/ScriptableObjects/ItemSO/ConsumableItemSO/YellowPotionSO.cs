using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Yellow Potion SO")]
public class YellowPotionSO : ConsumableItemSO
{
    public float speedBoost = 0.2f; // 20% speed boost
    public float duration = 20f;

    private void OnValidate()
    {
        description = "Consumable: Boost your move speed little for " + duration + " seconds (cannot stack)";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        player.MoveSpeedSystem.AddMoveSpeedMultiplier(speedBoost, duration);
    }
}
