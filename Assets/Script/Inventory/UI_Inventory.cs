using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour, ISelectItem {

    public event EventHandler<ISelectItem.OnItemSelectedEventArgs> OnItemSelected;

    [Header("UI Elements")]
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;
    [SerializeField] private ItemListSO itemListSO;

    [Header("Navigation")]
    [SerializeField] private UnityEngine.UI.Button closeButton;
    [SerializeField] private UnityEngine.UI.Button nextPageButton;
    [SerializeField] private UnityEngine.UI.Button previousPageButton;

    [Header("Actions")]
    [SerializeField] private UnityEngine.UI.Button useButton;
    [SerializeField] private UnityEngine.UI.Button dropButton;

    private Inventory inventory;
    private bool isInventoryShown;
    private int maxPage;
    private const int NumItemPerPage = 6;
    private int currentPage = 0;
    private Item selectedItem;

    private void Awake() {
        SetListener();
    }

    private void Start() {
        GameInput.Instance.OnInventoryAction += GameInput_OnInventoryAction;
        Hide();
    }

    private void SetListener() {
        closeButton.onClick.AddListener(() => {
            Hide();
        });
        nextPageButton.onClick.AddListener(() => {
            if (currentPage < maxPage) {
                currentPage++;
                RefreshInventoryItems();
            }
        });
        previousPageButton.onClick.AddListener(() => {
            if (currentPage > 0) {
                currentPage--;
                RefreshInventoryItems();
            }
        });

        useButton.onClick.AddListener(() => {
            if (selectedItem != null) {
                // Implement use item logic here
                Debug.Log($"Using item: {selectedItem.itemSO.itemName}");
            }
        });

        dropButton.onClick.AddListener(() => {
            if (selectedItem != null) {
                inventory.RemoveItem(selectedItem);
                if(selectedItem.quantity == 0) {
                    selectedItem = null; // Clear selection if item is fully removed
                }
                RefreshInventoryItems();
            }
        });
    }

    private void Hide() {
        gameObject.SetActive(false);
        isInventoryShown = false;
    }

    private void Show() {
        gameObject.SetActive(true);
        isInventoryShown = true;
        itemSlotTemplate.gameObject.SetActive(false);
        currentPage = 0;
        RefreshInventoryItems();
    }

    private void GameInput_OnInventoryAction(object sender, EventArgs e) {
        isInventoryShown = !isInventoryShown;
        if (isInventoryShown) {
            Show();
        }
        else {
            Hide();
        }
    }

    public void SetInventory(Inventory inventory) {
        if (itemListSO != null) {
            foreach (ItemSO itemSO in itemListSO.items) {
                Item item = new(itemSO);
                inventory.AddItem(item);
            }
            foreach (ItemSO itemSO in itemListSO.items) {
                Item item = new(itemSO);
                inventory.AddItem(item);
            }
        }
        this.inventory = inventory;
        currentPage = 0;
        RefreshInventoryItems();
    }

    private void RefreshInventoryItems() {
        RefreshInventoryItems(currentPage);
    }

    private void RefreshInventoryItems(int pageNumber) {
        int totalItems = inventory.GetItemListCount();
        maxPage = Mathf.Max(0, (totalItems - 1) / NumItemPerPage);

        // Clean previous slots
        foreach (Transform child in itemSlotContainer) {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }

        // Ensure valid page number
        pageNumber = Mathf.Clamp(pageNumber, 0, maxPage);

        int startIndex = pageNumber * NumItemPerPage;
        int endIndex = Mathf.Min(startIndex + NumItemPerPage, totalItems);

        int x = 0;
        int y = 0;
        List<Item> itemList = inventory.GetItemList();

        for (int i = startIndex; i < endIndex; i++) {
            Item item = itemList[i];
            RectTransform itemSlotTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotTransform.gameObject.SetActive(true);

            // Visuals
            UnityEngine.UI.Button button = itemSlotTransform.Find("ItemSlotButton").GetComponent<UnityEngine.UI.Button>();

            foreach (Transform child in button.transform) {
                Destroy(child.gameObject);
            }
            if (item.itemSO != null && item.itemSO.prefab != null) {
                Transform itemPrefab = Instantiate(item.itemSO.prefab, button.transform);
                itemPrefab.localPosition = new Vector3(0, 0, -1);
                itemPrefab.localScale = new Vector3(40, 40, 1);
            }

            // Button click logic for selection
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => {
                if(selectedItem == item) {
                    return;
                }
                selectedItem = item;
                RefreshInventoryItems(currentPage);

                OnItemSelected?.Invoke(this, new ISelectItem.OnItemSelectedEventArgs {
                    item = selectedItem
                });
            });

            // Quantity text
            GameObject quantityGameObject = itemSlotTransform.Find("QuantityText").gameObject;
            if (item.quantity > 1) {
                quantityGameObject.SetActive(true);
            } else {
                quantityGameObject.SetActive(false);
            }
            TextMeshProUGUI quantityText = quantityGameObject.transform.Find("foreground").GetComponent<TextMeshProUGUI>();
            quantityText.text = item.quantity.ToString();

            // Selected
            UnityEngine.UI.Image selectedImage = itemSlotTransform.Find("Selected").GetComponent<UnityEngine.UI.Image>();
            selectedImage.enabled = (selectedItem == item);

            x++;
            if (x > 3) {
                x = 0;
                y--;
            }
        }

        // disable/enable page buttons
        nextPageButton.interactable = currentPage < maxPage;
        previousPageButton.interactable = currentPage > 0;
    }
}
