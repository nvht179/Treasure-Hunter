using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.MPE;
using UnityEngine;

public class CrabbyVisual : MonoBehaviour
{
    [SerializeField] private Crabby crabby;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private static readonly int XVelocity = Animator.StringToHash("XVelocity");
    private static readonly int YVelocity = Animator.StringToHash("YVelocity");

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleFlipX();

        var velocity = crabby.GetVelocity();
        animator.SetFloat(XVelocity, math.abs(velocity.x));
        animator.SetFloat(YVelocity, math.abs(velocity.y));
    }

    private void HandleFlipX()
    {
        spriteRenderer.flipX = crabby.GetMoveDirection().x switch
        {
            > 0f => true,
            < 0f => false,
            _ => spriteRenderer.flipX
        };
    }
}