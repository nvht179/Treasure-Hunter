using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_Inventory : MonoBehaviour {

    private Inventory inventory;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private ItemListSO itemListSO;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private UnityEngine.UI.Button closeButton;

    private bool isShown;

    private void Awake() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        GameInput.Instance.OnInventoryAction += GameInput_OnInventoryAction;
        Hide();
    }

    private void Hide() {
        gameObject.SetActive(false);
        isShown = false;
    }

    private void Show() {
        gameObject.SetActive(true);
        isShown = true;
        itemSlotTemplate.gameObject.SetActive(false);
        RefreshInventoryItems();
    }

    private void GameInput_OnInventoryAction(object sender, EventArgs e) {
        isShown = !isShown;
        if (isShown) {
            Show();
        }
        else {
            Hide();
        }
    }

    public void SetInventory(Inventory inventory) {
        if (itemListSO != null) {
            foreach (ItemSO itemSO in itemListSO.items) {
                Item item = new Item(itemSO);
                inventory.AddItem(item);
            }
        }
        this.inventory = inventory;
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems() {
        foreach(Transform child in itemSlotContainer) {
            if(child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        int x = 0;
        int y = 0;
        float itemSlotCellSize = 132f;
        foreach (Item item in inventory.GetItemList()) {
            RectTransform itemSlotTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotTransform.gameObject.SetActive(true);
            itemSlotTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            
            UnityEngine.UI.Button button = itemSlotTransform.Find("ItemSlotButton").GetComponent<UnityEngine.UI.Button>();

            // clear previous prefab
            foreach (Transform child in button.transform) {
                Destroy(child.gameObject);
            }
            // init prefab as child of image
            if (item.itemSO != null && item.itemSO.prefab != null) {
                Transform itemPrefab = Instantiate(item.itemSO.prefab, button.transform);
                itemPrefab.localPosition = new Vector3(0, 0, -1);
                itemPrefab.localScale = new Vector3(40, 40, 1);
            }

            description.text = item.itemSO != null ? item.itemSO.itemName : "No Item";
            x++;
            if(x > 3) {
                x = 0;
                y--;
            }
        }
    }
}
