using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyInfo : MonoBehaviour {
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI moneyText;

    public void Start() {
        player.OnGoldChanged += Player_OnGoldChanged;
        moneyText.text = player.GetMoney().ToString();
    }

    private void Player_OnGoldChanged(object sender, Player.OnGoldChangedEventArgs e) {
        int money = e.currentGold + e.changeAmount;
        moneyText.text = money.ToString();
    }
}
