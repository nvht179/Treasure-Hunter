using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image barImage;

    public void Start()
    {
        player.HealthSystem.OnHealthChanged += HealthSystemOnHealthChanged;
        barImage.fillAmount = 1;
    }

    private void HealthSystemOnHealthChanged(object sender, HealthSystem.OnHealthChangedEventArgs e)
    {
        var healthNormalized = e.CurrentHealth / e.MaxHealth;
        barImage.fillAmount = healthNormalized;
    }
}
