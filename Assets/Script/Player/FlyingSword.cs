using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSword : MonoBehaviour
{
    [SerializeField] private FlyingObjectSO flyingObjectSO;
    [SerializeField] private LayerMask collisionLayer;

    private Rigidbody2D rb;
    public int Direction { set; get; }
    private bool isEmbedded;
    private float destroyTimer;
    private const float DestroyTime = 0.5f; // duration of embedded sword animation

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(flyingObjectSO.speed * Direction, 0);
        isEmbedded = false;
        destroyTimer = DestroyTime;
    }

    private void Update()
    {
        rb.velocity = new Vector3(flyingObjectSO.speed * Direction, 0);
        
        if (isEmbedded)
        {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((collisionLayer & (1 << other.gameObject.layer)) != 0) {
            rb.velocity = Vector2.zero;
            var hitPlayer = other.collider.GetComponent<IDamageable>();
            hitPlayer?.TakeDamage(flyingObjectSO.damage);
            transform.SetParent(other.transform);
            isEmbedded = true;
        }
    }

    public bool IsEmbedded()
    {
        return isEmbedded;
    }

    public Vector2 GetVelocity()
    {
        return rb.velocity;
    }
}
