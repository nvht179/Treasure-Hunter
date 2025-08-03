using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Sword")]
public class Sword : PassiveItemSO
{
    public float damageAmount = 1;
    private void OnValidate()
    {
        description = "Passive: Provides " + damageAmount.ToString() + " damage";
        droppable = true;
        consumable = false;
    }
    public override void ApplyEffect(Player player)
    {
    }

    public override void RemoveEffect(Player player)
    {
    }
}