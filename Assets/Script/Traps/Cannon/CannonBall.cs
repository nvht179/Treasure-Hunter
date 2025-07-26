using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public event EventHandler OnCannonBallCollided;

    [SerializeField] private FlyingObjectSO cannonBallSO;
    [SerializeField] private LayerMask collisionLayer;
    
    private Rigidbody2D rb;
    private Vector2 direction;
    private bool willDestroy;
    private float destroyTimer;
    private const float DestroyDelay = 0.3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * cannonBallSO.speed;
        }

        willDestroy = false;
    }

    private void Update()
    {
        if (willDestroy)
        {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= DestroyDelay)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collisionLayer & (1 << collision.gameObject.layer)) != 0)
        {
            OnCannonBallCollided?.Invoke(this, EventArgs.Empty);
            rb.velocity = Vector2.zero;
            willDestroy = true;
            destroyTimer = 0f;
            var hitPlayer = collision.GetComponent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(cannonBallSO.damage);
            }
        }
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    public FlyingObjectSO GetFlyingObjectSO()
    {
        return cannonBallSO;
    }
}