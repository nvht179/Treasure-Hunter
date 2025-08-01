using System;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractiveObject {

    public event EventHandler<OnShopInteractEventArgs> OnInteract;
    public class OnShopInteractEventArgs : EventArgs {
        public Player player;
    }

    // Sound effect
    public event EventHandler OnShopOpen;
    public event EventHandler OnShopClose;

    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ItemListSO shopItemListSO;

    private Inventory shopInventory;

    private void Awake() {
        shopInventory = new Inventory();
        foreach (ItemSO itemSO in shopItemListSO.items) {
            int appearChance = UnityEngine.Random.Range(0, 100);
            if(appearChance < 50) {
                continue; // 50% chance to not appear
            }
            if (itemSO.stackable) {
                int randomStackSize = UnityEngine.Random.Range(1, itemSO.maxStack + 1);
                shopInventory.AddItem(new Item(itemSO, randomStackSize));
            } else {
                shopInventory.AddItem(new Item(itemSO, 1));
            }
        }
    }

    public void Interact(Player player) {
        OnInteract?.Invoke(this, new OnShopInteractEventArgs {
            player = player
        });
        if (!shopUI.IsOpened()) {
            shopUI.OpenShopUI(player, shopInventory);
            OnShopOpen?.Invoke(this, EventArgs.Empty);
        }
        else {
            shopUI.CloseShopUI();
            OnShopClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
