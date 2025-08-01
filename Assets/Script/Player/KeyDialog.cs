using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDialog : MonoBehaviour {
    [SerializeField] private Transform keyDialogVisual;
    [SerializeField] private Player player;

    private const float dialogDuration = 2f;
    private float dialogTimer;

    private void Awake() {
        keyDialogVisual.gameObject.SetActive(false);
    }

    private void Start() {
        player.OnNeedKey += Player_OnNeedKey;
    }

    private void Update() {
        if (dialogTimer > 0) {
            dialogTimer -= Time.deltaTime;
            if (dialogTimer <= 0) {
                keyDialogVisual.gameObject.SetActive(false);
            }
        }
    }

    private void Player_OnNeedKey(object sender, EventArgs e) {
        keyDialogVisual.gameObject.SetActive(true);
        dialogTimer = dialogDuration;
    }
}
