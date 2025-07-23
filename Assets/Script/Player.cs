using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform playerPivot;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 2f;

    [Header("References")]
    [SerializeField] private GameInput gameInput;

    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private LayerMask groundLayer;

    private const float GroundCheckRadius = 0.01f;

    private bool isGrounded;
    private bool isRunning;
    private Vector3 moveDir;
    public bool IsFacingRight { get; set; }
    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        IsFacingRight = true;
    }

    private void Start()
    {
        gameInput.OnAttackAction += HandleAttack;
        gameInput.OnJumpAction += HandleJump;
    }

    private void Update()
    {
        // handle input
        var inputVector = gameInput.GetMovementVectorNormalized();
        moveDir = new Vector3(inputVector.x, inputVector.y, 0);

        // handle actions
        HandleRunning();
        playerVisual.HandleFlipX(this);

        // update states
        isRunning = moveDir.x != 0f;
        isGrounded = Physics2D.OverlapCircle(playerPivot.position, GroundCheckRadius, groundLayer);
    }

    private void HandleJump(object sender, EventArgs e)
    {
        if (isGrounded)
        {
            var gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
            var jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }
    }

    private void HandleRunning()
    {
        var moveDistance = moveSpeed * Time.deltaTime;
        var moveDirX = new Vector3(moveDir.x, 0, 0);
        transform.position += moveDistance * moveDirX;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDir;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsJumping()
    {
        return !isGrounded && moveDir.y > 0f;
    }

    public bool IsFalling()
    {
        return !isGrounded && moveDir.y < 0f;
    }

    // public bool IsGroundedByRaycast() {
    //     // origin at the bottom of the collider
    //     Vector2 origin = new Vector2(transform.position.x, GetComponent<BoxCollider2D>().bounds.min.y);
    //     RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, groundLayer);
    //     return hit.collider != null;
    // }

    private void HandleAttack(object sender, EventArgs e)
    {
        // Pick one of three attack animations (0, 1 or 2) for placeholder
        _ = UnityEngine.Random.Range(0, 3);
        // TODO: Trigger attack animation based on attackIndex
    }

    public bool IsHit()
    {
        return false; // Placeholder for hit detection logic
    }

    public bool IsDeadHit()
    {
        return false; // Placeholder for dead hit detection logic
    }
}