using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Blue Potion SO")]
public class BluePotionSO : ConsumableItemSO
{
    public float staminaBoost = 5f;
    public float boostDuration = 5f;

    private void OnValidate()
    {
        description = "Boosts stamina by " + staminaBoost.ToString() + " for " + boostDuration.ToString() + " seconds.";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player)
    {
        player.GetStaminaSystem().ApplyBonusRegen(staminaBoost, boostDuration);
    }
}
