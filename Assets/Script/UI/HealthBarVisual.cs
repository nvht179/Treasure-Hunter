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
        player.OnDamageTaken += PlayerOnDamageTaken;
        barImage.fillAmount = 1;
    }

    private void PlayerOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
    {
        var healthNormalized = e.CurrentHealth / e.MaxHealth;
        barImage.fillAmount = healthNormalized;
    }
}
