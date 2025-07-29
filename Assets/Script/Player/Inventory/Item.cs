using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {

    public ItemSO itemSO;
    public int quantity;

    public Item(ItemSO itemSO)
    {
        this.itemSO = itemSO;
        this.quantity = 1;
    }

    public Item(ItemSO itemSO, int quantity)
    {
        this.itemSO = itemSO;
        this.quantity = quantity;
    }

}
