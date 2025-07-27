using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectOrigin : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameInput gameInput;

    private Animator animator;
    private bool isGrounded;
    private bool wasGrounded;
    private bool hasJumped;
    private float resetJumpEffectTimer;

    private const float ResetJumpEffectTime = 0.5f;
    private const float FallVelocityThreshold = -10f;

    private static readonly int Landed = Animator.StringToHash("Landed");
    private static readonly int Jumped = Animator.StringToHash("Jumped");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        resetJumpEffectTimer = ResetJumpEffectTime;
        isGrounded = false;
        wasGrounded = false;
        hasJumped = false;
    }

    private void Start()
    {
        gameInput.OnJumpAction += PlayerEffectOriginOnJumpAction;
    }

    private void PlayerEffectOriginOnJumpAction(object sender, EventArgs e)
    {
        hasJumped = true;
    }

    private void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = player.IsGrounded();

        resetJumpEffectTimer -= Time.deltaTime;

        HandleLanding();
        HandleJumping();
    }

    private void HandleJumping()
    {
        if (hasJumped && resetJumpEffectTimer < 0)
        {
            transform.position = player.transform.position;
            animator.SetTrigger(Jumped);
            resetJumpEffectTimer = ResetJumpEffectTime;
            hasJumped = false;
        }
        else if (hasJumped && resetJumpEffectTimer > 0)
        {
            hasJumped = false;
            resetJumpEffectTimer = ResetJumpEffectTime;
        }
    }

    private void HandleLanding()
    {
        var playerVelocity = player.GetVelocity();
        if (!wasGrounded && isGrounded && playerVelocity.y < FallVelocityThreshold)
        {
            transform.position = player.transform.position;
            animator.SetTrigger(Landed);
        }
    }
}