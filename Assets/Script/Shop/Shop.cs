using System;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractiveObject {

    public event EventHandler<OnShopInteractEventArgs> OnInteract;
    public class OnShopInteractEventArgs : EventArgs {
        public Player player;
    }

    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ItemListSO shopItemListSO;

    public void Interact(Player player) {
        OnInteract?.Invoke(this, new OnShopInteractEventArgs {
            player = player
        });
        shopUI.OpenShopUI(player, shopItemListSO);
    }
}
