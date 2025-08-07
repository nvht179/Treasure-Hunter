using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Black Potion SO")]
public class BlackPotionSO : ConsumableItemSO
{
    public float goldMultiplier = 0.5f; // 50% more gold
    public float duration = 60f;

    private void OnValidate()
    {
        description = "Consumable: Earn more " + goldMultiplier.ToString() + " gold multiplier in " + duration.ToString() + " seconds but you will receive double damage.";
        droppable = true;
        consumable = true;
    }

    public override void Consume(Player player, out float duration)
    {
        duration = this.duration;
        player.DamageReceivedSystem.AddDamageReceivedBuff(2.0f, duration);
        player.GoldBonusSystem.AddGoldBonus(goldMultiplier, duration);
    }
}
