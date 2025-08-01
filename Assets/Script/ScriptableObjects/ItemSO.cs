using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemData.cs
[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject {
    public string itemName;
    public ItemType itemType;
    public string description;
    public Sprite icon;
    public Transform prefab;

    public bool consumable;

    public bool stackable;
    public int maxStack;

    public int buyPrice;
}

public enum ItemType {
    BlueDiamond,
    BluePotion,
    GoldCoin,
    GoldenSkull,
    GreenDiamond,
    GreenPotion,
    HealthPotion,
    RedDiamond,
    SilverCoin,
}