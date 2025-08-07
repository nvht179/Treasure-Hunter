using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ShopUI shopUI;

    private void Start()
    {
        inventoryUI.OnInventoryOpen += InventoryUI_OnInventoryOpen;
        shopUI.OnShopOpen += ShopUI_OnShopOpen;
    }

    private void ShopUI_OnShopOpen(object sender, EventArgs e)
    {
        if (inventoryUI.IsInventoryShown())
        {
            inventoryUI.Hide();
        }
    }

    private void InventoryUI_OnInventoryOpen(object sender, EventArgs e)
    {
        if(shopUI.IsOpened())
        {
            shopUI.CloseShopUI();
        }
    }
}
