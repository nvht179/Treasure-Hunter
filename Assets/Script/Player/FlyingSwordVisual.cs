using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FlyingSwordVisual : MonoBehaviour
{
    [SerializeField] private FlyingSword flyingSword;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private int direction;
    
    private static readonly int IsEmbedded = Animator.StringToHash("IsEmbedded");
    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        direction = flyingSword.Direction;
        HandleFlipX();
        var isEmbedded = flyingSword.IsEmbedded();
        var velocity = flyingSword.GetVelocity();
        
        animator.SetBool(IsEmbedded, isEmbedded);
        animator.SetFloat(XVelocity, math.abs(velocity.x));
    }
    
    private void HandleFlipX()
    {
        spriteRenderer.flipX = direction switch
        {
            1 => false,
            -1 => true,
            _ => spriteRenderer.flipX
        };
    }
}
