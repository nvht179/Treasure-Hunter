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

    public void HandleFlipX(Vector3 moveDir)
    {
        spriteRenderer.flipX = moveDir.x switch
        {
            > 0f => false,
            < 0f => true,
            _ => spriteRenderer.flipX
        };
    }
}
