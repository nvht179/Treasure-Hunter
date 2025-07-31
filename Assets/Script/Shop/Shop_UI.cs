using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour, ISelectItem {

    public event EventHandler<ISelectItem.OnItemSelectedEventArgs> OnItemSelected;

    [Header("Shop Settings")]
    [SerializeField] private Shop shop;
    [SerializeField] private Transform shopVisuals;
    [SerializeField] private ItemListSO shopItemListSO;

    [Header("UI Elements")]
    [SerializeField] private Transform itemSlotContainer;
    [SerializeField] private Transform itemSlotTemplate;

    [Header("Navigation")]
    [SerializeField] private UnityEngine.UI.Button closeButton;
    [SerializeField] private UnityEngine.UI.Button nextPageButton;
    [SerializeField] private UnityEngine.UI.Button previousPageButton;

    [Header("Actions")]
    [SerializeField] private UnityEngine.UI.Button buyButton;

    private const int NumItemPerPage = 6;
    private Inventory playerInventory;
    private Inventory shopInventory;
    private int maxPage;
    private int currentPage = 0;
    private Item selectedItem;

    private void Awake() {
        SetupShop();
        RefreshInventoryItems();
        Hide();
    }

    private void Start() {
        shop.OnInteract += ShopOnInteract;
    }

    private void ShopOnInteract(object sender, Shop.OnShopInteractEventArgs e) {
        SetInventory(e.player.GetInventory());
        Show();
    }

    private void SetupShop() {
        shopInventory = new Inventory();
        foreach (ItemSO itemSO in shopItemListSO.items) {
            shopInventory.AddItem(new Item(itemSO));
        }
        SetupListener();
    }

    private void SetupListener() {
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

        buyButton.onClick.AddListener(() => {
            if (selectedItem == null) return;
            // Check if player can afford the item
            if (playerInventory.GetItemListCount() < selectedItem.itemSO.buyPrice) {
                // TODO: Show Message to player that they can't afford the item
                return;
            }
            // Add item to player's inventory
            playerInventory.AddItem(new Item(selectedItem.itemSO, 1));
            shopInventory.RemoveItem(selectedItem);
            selectedItem = null;
            RefreshInventoryItems();
        });

    }

    private void Hide() {
        shopVisuals.gameObject.SetActive(false);
    }

    private void Show() {
        currentPage = 0;
        RefreshInventoryItems();
        shopVisuals.gameObject.SetActive(true);
        itemSlotTemplate.gameObject.SetActive(false);
    }

    private void SetInventory(Inventory inventory) {
        playerInventory = inventory;
    }

    private void RefreshInventoryItems() {
        RefreshInventoryItems(currentPage);
    }

    private void RefreshInventoryItems(int pageNumber) {
        int totalItems = shopItemListSO.items.Count;
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
        List<Item> itemList = shopInventory.GetItemList();

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
