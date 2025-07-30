using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBarVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image barImage;

    public void Start()
    {
        player.OnStaminaUsed += PlayerOnStaminaUsed;
        barImage.fillAmount = 1;
    }

    private void PlayerOnStaminaUsed(object sender, Player.OnStaminaUsedEventArgs e)
    {
        var staminaNormalized = e.CurrentStamina / e.MaxStamina;
        barImage.fillAmount = staminaNormalized;
    }
}
