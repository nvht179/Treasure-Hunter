using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveItemSO : ItemSO, IPassiveEffect
{
    public abstract void ApplyEffect(Player player);
    public abstract void RemoveEffect(Player player);
}