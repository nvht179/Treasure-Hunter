using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour, ISelectItem {

    public event EventHandler OnShopOpen;
    public event EventHandler<ISelectItem.OnItemSelectedEventArgs> OnItemSelected;
    public event EventHandler OnNotEnoughMoney;

    [Header("Shop Settings")]
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

    private Player player;
    private bool isOpened = false;
    private const int NumItemPerPage = 6;
    private Inventory playerInventory;
    private Inventory shopInventory;
    private int maxPage;
    private int currentPage = 0;
    private Item selectedItem;

    public event EventHandler<OnItemBuyEventArgs> OnItemBuy;
    public class OnItemBuyEventArgs : EventArgs {
        public Item item;
    }

    private void Awake() {
        SetupListener();
        Hide();
    }

    public void OpenShopUI(Player player, Inventory inventory) {
        this.player = player;
        shopInventory = inventory;
        RefreshInventoryItems();
        SetInventory(player.GetInventory());
        Show();
        OnShopOpen?.Invoke(this, EventArgs.Empty);
    }

    public void CloseShopUI() {
        Hide();
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
            if (player.GetMoney() < selectedItem.itemSO.buyPrice) {
                OnNotEnoughMoney?.Invoke(this, EventArgs.Empty);
                return;
            }
            // Add item to player's inventory
            playerInventory.AddItem(new Item(selectedItem.itemSO, 1));
            shopInventory.RemoveItem(selectedItem);

            OnItemBuy?.Invoke(this, new OnItemBuyEventArgs {
                item = selectedItem
            });
            if(selectedItem.quantity == 0) {
                selectedItem = null;
            }
            OnItemSelected?.Invoke(this, new ISelectItem.OnItemSelectedEventArgs {
                item = selectedItem
            });
            RefreshInventoryItems();
        });

    }

    private void Hide() {
        isOpened = false;
        shopVisuals.gameObject.SetActive(false);
    }

    private void Show() {
        currentPage = 0;
        isOpened = true;
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
        List<Item> itemList = shopInventory.GetItemList();
        int totalItems = shopInventory.GetItemListCount();
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
                itemPrefab.localScale = new Vector3(item.itemSO.scaleInUI, item.itemSO.scaleInUI, 1);
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

    public bool IsOpened() {
        return isOpened;
    }
}
