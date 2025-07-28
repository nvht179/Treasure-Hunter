using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item List")]
public class ItemListSO : ScriptableObject {
    public List<ItemSO> items;
}
