using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    private const float MaxWidth = 235;
    private RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        player.OnDamageTaken += PlayerOnDamageTaken;
    }

    private void PlayerOnDamageTaken(object sender, Player.OnDamageTakenEventArgs e)
    {
        var width = MaxWidth * (e.CurrentHealth / e.MaxHealth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
