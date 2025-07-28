using UnityEngine;

public class Crabby : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform groundAndWallCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private int moveDirection = 1; // 1 = right, -1 = left
    
    private const float WallCheckDistance = 0.8f;
    private const float GroundCheckDistance = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);

        if (IsGroundAhead() || IsHittingWall())
        {
            moveDirection *= -1;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private bool IsGroundAhead()
    {
        var hit = Physics2D.Raycast(groundAndWallCheck.position, Vector2.down, GroundCheckDistance, groundLayer);
        return hit.collider == null;
    }

    private bool IsHittingWall()
    {
        var dir = moveDirection == 1 ? Vector2.right : Vector2.left;
        var hit = Physics2D.Raycast(groundAndWallCheck.position, dir, WallCheckDistance, groundLayer);
        return hit.collider != null;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection == 1 ? Vector3.right : Vector3.left;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    private void OnDrawGizmos()
    {
        if (groundAndWallCheck != null)
        {
            var dir = moveDirection == 1 ? Vector2.right : Vector2.left;
            Gizmos.DrawRay(groundAndWallCheck.position, Vector2.down * GroundCheckDistance);
            Gizmos.DrawRay(groundAndWallCheck.position, dir * WallCheckDistance);
        }
    }
}