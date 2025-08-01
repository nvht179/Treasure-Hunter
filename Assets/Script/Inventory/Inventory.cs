using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    private List<Item> itemList;
    private readonly Action<Item> useItemAction;
    private readonly Action<Item> wearItemAction;
    private readonly Action<Item> dropItemAction;

    public Inventory(Action<Item> useItemAction = null, Action<Item> wearItemAction = null, Action<Item> dropItemAction = null) {
        this.useItemAction = useItemAction;
        this.wearItemAction = wearItemAction;
        this.dropItemAction = dropItemAction;
        itemList = new List<Item>();
    }

    public void AddItem(Item newItem) {
        if (newItem == null || newItem.itemSO == null) return;

        if (newItem.itemSO.stackable) {
            // Try to find an existing stack
            foreach (Item item in itemList) {
                if (item.itemSO == newItem.itemSO) {
                    int total = item.quantity + newItem.quantity;
                    if (total <= newItem.itemSO.maxStack) {
                        item.quantity = total;
                        return;
                    }
                    else {
                        item.quantity = newItem.itemSO.maxStack;
                        newItem.quantity = total - newItem.itemSO.maxStack;
                        // Try adding remaining as new stack
                        AddItem(newItem);
                        return;
                    }
                }
            }
        }

        // If not stackable or no existing stack found or excess beyond maxStack
        itemList.Add(new Item(newItem.itemSO, newItem.quantity));
        wearItemAction?.Invoke(newItem);
    }

    public void RemoveItem(Item itemToRemove) {
        if (itemToRemove == null || itemToRemove.itemSO == null) return;
        for (int i = 0; i < itemList.Count; i++) {
            Item item = itemList[i];
            if (item.itemSO == itemToRemove.itemSO) {
                if (item.quantity > 1) {
                    --item.quantity;
                } else {
                    itemList.RemoveAt(i);
                }
                return;
            }
        }
        dropItemAction?.Invoke(itemToRemove);
    }

    public void UseItem(Item item) {
        useItemAction(item);
    }

    public List<Item> GetItemList() {
        return itemList;
    }

    public int GetItemListCount() {
        return itemList.Count;
    }
}
