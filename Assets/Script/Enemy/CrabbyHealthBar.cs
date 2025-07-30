using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrabbyHealthBar : MonoBehaviour
{
    [SerializeField] private Crabby crabby;
    [SerializeField] private Image barImage;

    private void Start()
    {
        crabby.OnDamageTaken += CrabbyOnDamageTaken;
        barImage.fillAmount = 1;
    }

    private void CrabbyOnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e)
    {
        var healthNormalized = e.CurrentHealth / e.MaxHealth;
        barImage.fillAmount = healthNormalized;

        if (healthNormalized == 0f || Mathf.Approximately(healthNormalized, 1f))
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}