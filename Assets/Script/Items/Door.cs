using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractiveObject {
    public event EventHandler OnDoorInteracted;

    [SerializeField] private Player player;

    public void Interact(Player player) {
        if(player.HasKey()) {
            // win
            OnDoorInteracted?.Invoke(this, EventArgs.Empty);
        } else {
            player.ActivateNeedKeyDialog();
        }
    }
}
