using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabbyVisual : MonoBehaviour
{
    
    [SerializeField] private Crabby crabby;
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleFlipX();
    }

    private void HandleFlipX()
    {
        switch (crabby.GetMoveDirection().x)
        {
            case > 0f:
                spriteRenderer.flipX = false;
                isFacingRight = true;
                break;
            case < 0f:
                spriteRenderer.flipX = true;
                isFacingRight = false;
                break;
        }
    }
}
