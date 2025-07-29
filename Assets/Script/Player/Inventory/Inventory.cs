using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    private List<Item> itemList;

    public Inventory() {
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
    }

    public List<Item> GetItemList() {
        return itemList;
    }

    public int GetItemListCount() {
        return itemList.Count;
    }
}
