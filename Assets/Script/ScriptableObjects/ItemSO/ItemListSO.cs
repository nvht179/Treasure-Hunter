using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item List SO")]
public class ItemListSO : ScriptableObject {
    public List<ItemSO> items;
}
