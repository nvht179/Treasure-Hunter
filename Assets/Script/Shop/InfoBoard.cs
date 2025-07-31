using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoBoard : MonoBehaviour {
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private Transform infoBoardVisual;
    [SerializeField] private Button okButton;

    private void Awake() {
        okButton.onClick.AddListener(Hide);
        Hide();
    }

    private void Start() {
        shopUI.OnNotEnoughMoney += ShopUI_OnNotEnoughMoney;
    }

    private void ShopUI_OnNotEnoughMoney(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        infoBoardVisual.gameObject.SetActive(true);
    }

    private void Hide() {
        infoBoardVisual.gameObject.SetActive(false);
    }
}
