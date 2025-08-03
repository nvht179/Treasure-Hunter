using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Passive Items/Shield")]
public class Shield : PassiveItemSO
{
    public float shieldAmount = 1;
    private void OnValidate()
    {
        description = "Passive: Provides " + shieldAmount.ToString() + " shield points.";
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