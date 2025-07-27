using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackEffect : MonoBehaviour
{
    [SerializeField] private Player player;

    private Animator animator;
    private Vector3 direction;
    private SpriteRenderer spriteRenderer;

    private static readonly int AirAttacked = Animator.StringToHash("AirAttacked");
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        direction = player.GetMoveDirection();
        
        HandleAirAttacking();
        HandleFlipX();
    }

    private void HandleAirAttacking()
    {
        if (player.IsAttacking() && !player.IsGrounded())
        {
            transform.position = new Vector3(transform.position.x,
                transform.position.y - transform.localPosition.y, transform.position.z);
            animator.SetTrigger(AirAttacked);
        }
    }

    private void HandleFlipX()
    {
        spriteRenderer.flipX = direction.x switch
        {
            > 0f => false,
            < 0f => true,
            _ => spriteRenderer.flipX
        };
    }
}