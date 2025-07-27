using UnityEngine;

public class Crabby : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform groundAndWallCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private int moveDirection = 1; // 1 = right, -1 = left

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);
        
        if (!IsGroundAhead() || IsHittingWall())
        {
            moveDirection *= -1;
        }
    }

    private bool IsGroundAhead()
    {
        var hit = Physics2D.Raycast(groundAndWallCheck.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private bool IsHittingWall()
    {
        var dir = moveDirection == 1 ? Vector2.right : Vector2.left;
        var hit = Physics2D.Raycast(groundAndWallCheck.position, dir, 0.1f, groundLayer);
        return hit.collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundAndWallCheck != null)
        {
            Gizmos.DrawRay(groundAndWallCheck.position, Vector2.down * 0.1f);
            Gizmos.DrawRay(groundAndWallCheck.position, (moveDirection == 1 ? Vector2.right : Vector2.left) * 0.1f);
        }
    }
}