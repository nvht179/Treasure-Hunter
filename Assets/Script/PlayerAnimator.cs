using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private const string RUN = "Run";
    private const string IDLE = "Idle";
    private const string JUMP = "Jump";
    private const string FALL = "Fall";
    private const string HIT = "Hit";
    private const string DEADHIT = "DeadHit";
    private const string ATTACK = "Attack";

    [SerializeField] private Player player;
    private Animator animator;

    // Keep track of the grounded state from the previous frame:
    private bool wasGrounded;

    private void Awake() {
        animator = GetComponent<Animator>();
        wasGrounded = false;
    }

    private void Update() {
        // 1) Read current states from your Player
        bool isRunning = player.IsRunning();
        bool isJumping = player.IsJumping();
        bool isFalling = player.IsFalling();
        bool isHit = player.IsHit();
        bool isDeadHit = player.IsDeadHit();
        //int attackIndex = player.GetLastAttackIndex();

        // 2) Update simple booleans/triggers
        animator.SetBool(RUN, isRunning);
        animator.SetBool(JUMP, isJumping);
        animator.SetBool(FALL, isFalling);

        if (isHit)
            animator.SetTrigger(HIT);

        if (isDeadHit)
            animator.SetTrigger(DEADHIT);

        //animator.SetInteger(ATTACK, attackIndex);
    }
}
