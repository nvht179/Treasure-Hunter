using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ItemData.cs
[CreateAssetMenu(menuName = "Items/ItemSO")]
public class ItemSO : ScriptableObject {
    public string itemName;
    public string description;
    public bool droppable;
    public bool consumable;
    public ItemType itemType;

    public Sprite icon;
    public Transform prefab;
    public Transform collectEffectPrefab;
    public float scaleInUI = 20;

    public bool stackable;
    public int maxStack;

    public int buyPrice;
}

public enum ItemType {
    Sword,
    Shield,
    Hat,
    Boots,
    Ring,
    GoldenKey,
    Diamond,
    Resources,
    Consumable,
}