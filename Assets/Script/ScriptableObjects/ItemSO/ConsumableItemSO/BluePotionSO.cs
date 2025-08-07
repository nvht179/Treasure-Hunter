using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Blue Potion SO")]
public class BluePotionSO : ConsumableItemSO
{
    public float rate = 1f;
    public float duration = 5f;

    private void OnValidate()
    {
        description = "Consumable: Increase " + rate.ToString() + " stamina restored each second in " + duration.ToString() + " seconds";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player, out float duration)
    {
        duration = this.duration;
        player.StaminaSystem.ApplyBonusRegen(rate, duration);
    }
}