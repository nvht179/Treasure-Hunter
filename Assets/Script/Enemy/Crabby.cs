using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crabby : MonoBehaviour
{
    // Idling Crabby stands still and pursuing player when in sight
    // Wandering Crabby wanders until hitting a collider then turn back
    // Jumping Crabby jumps in place
    // Crabby starts attacking when player within attack range
    private enum MovementOption
    {
        Idling,
        Wandering,
        Jumping
    }

    [SerializeField] private MovementOption movementOption;
    [SerializeField] private Transform characterPivotPoint;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private const float GroundCheckRadius = 0.1f;
    private const float AttackRange = 1f;
    private const float JumpHeight = 5f;
    private const float MoveSpeed = 5f;
    private const float RunSpeed = 10f;
    
    private bool isAttacking;
    private bool isPursuing;
    private bool isJumping;
    private Vector3 moveDir;

    private void Awake()
    {
        moveDir = UnityEngine.Random.value > 0.5f ? Vector3.left : Vector3.right;
        rb = GetComponent<Rigidbody2D>();
        if (movementOption == MovementOption.Wandering)
        {
            rb.velocity = new Vector2(moveDir.x * MoveSpeed, 0);
        }
    }

    private void Update()
    {
        HandleIdle();
    }

    public Vector3 GetMoveDirection()
    {
        return Vector3.zero;
    }

    private void HandleIdle()
    {
        if (isAttacking || isPursuing) return;
        switch (movementOption)
        {
            case MovementOption.Idling:
                break;
            case MovementOption.Jumping:
                if (Physics2D.OverlapCircle(characterPivotPoint.position, GroundCheckRadius, groundLayer))
                {
                    var gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
                    var jumpVelocity = Mathf.Sqrt(2 * gravity * JumpHeight);
                    rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
                    moveDir = Vector3.up;
                }

                break;
            case MovementOption.Wandering:
                const float hitDistance = 0.2f;
                var hit = Physics2D.Raycast(transform.position, moveDir, hitDistance, groundLayer);
                if (hit.collider != null)
                {
                    moveDir = - moveDir;
                }
                break;
        }
    }
}