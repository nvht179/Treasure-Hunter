using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HandleFlipX(Player player)
    {
        switch (player.GetMoveDirection().x)
        {
            case > 0f:
                spriteRenderer.flipX = false;
                player.IsFacingRight = true;
                break;
            case < 0f:
                spriteRenderer.flipX = true;
                player.IsFacingRight = false;
                break;
        }
    }
}