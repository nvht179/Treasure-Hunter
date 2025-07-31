using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemData.cs
[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject {
    public string itemName;
    public string description;
    public Sprite icon;
    public Transform prefab;
    public ItemType type;
    public bool stackable;
    public int maxStack;

    public int buyPrice;
}

public enum ItemType { Weapon, Potion, Resource }
