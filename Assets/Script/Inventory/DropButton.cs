using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropButton : MonoBehaviour {
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform selectectVisual;

    private void Awake() {
        Hide();
    }

    private void Start() {
        inventoryUI.OnItemSelected += InventoryUI_OnItemSelected;
    }

    private void InventoryUI_OnItemSelected(object sender, ISelectItem.OnItemSelectedEventArgs e) {
        if(e.item == null || e.item.itemSO == null) {
            Hide();
            return;
        }
        if (e.item.itemSO.droppable) {
            Show();
        }
        else {
            Hide();
        }
    }

    private void Show() {
        visual.gameObject.SetActive(true);
        selectectVisual.gameObject.SetActive(false);
    }

    private void Hide() {
        visual.gameObject.SetActive(false);
        selectectVisual.gameObject.SetActive(true);
    }
}
