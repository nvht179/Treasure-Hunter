using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour, IInteractiveObject {

    public event EventHandler<OnShopInteractEventArgs> OnInteract;
    public class OnShopInteractEventArgs : EventArgs {
        public Player player;
    }

    [SerializeField] private ShopUI shopUI;

    public void Interact(Player player) {
        OnInteract?.Invoke(this, new OnShopInteractEventArgs {
            player = player
        });
    }
}
