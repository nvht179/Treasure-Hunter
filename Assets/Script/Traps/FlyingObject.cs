using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObject : MonoBehaviour {

    [SerializeField] protected FlyingObjectSO flyingObjectSO;
    [SerializeField] protected LayerMask collisionLayer;

    protected Rigidbody2D rb;
    protected Vector2 direction;
    protected bool willDestroy;
    protected float destroyTimer;
    protected float destroyDelay;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.velocity = direction * flyingObjectSO.speed;
        }

        willDestroy = false;
    }

    private void Update() {
        if (willDestroy) {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= destroyDelay) {
                Destroy(gameObject);
            }
        }
    }

    protected void StopFlying() {
        rb.velocity = Vector2.zero;
        willDestroy = true;
        destroyTimer = 0f;
    }

    protected void DamageObject(IDamageable damageable) {
        damageable?.TakeDamage(this, flyingObjectSO.damage);
    }

    public void SetDirection(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    public FlyingObjectSO GetFlyingObjectSO() {
        return flyingObjectSO;
    }
}
