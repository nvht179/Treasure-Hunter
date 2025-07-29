using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour {

    private Inventory inventory;
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private ItemListSO itemListSO;
    [SerializeField] private TextMeshProUGUI description;

    private void Awake() {
        itemSlotTemplate.gameObject.SetActive(false);
    }

    public void SetInventory(Inventory inventory) {
        if (itemListSO != null)
        {
            foreach (ItemSO itemSO in itemListSO.items)
            {
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
            
            Image image = itemSlotTransform.Find("image").GetComponent<Image>();

            // clear previous prefab
            foreach (Transform child in image.transform) {
                Destroy(child.gameObject);
            }
            // init prefab as child of image
            if (item.itemSO != null && item.itemSO.prefab != null) {
                Transform itemPrefab = Instantiate(item.itemSO.prefab, image.transform);
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
