using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {
    public event EventHandler OnCannonBallCollided;

    [SerializeField] private FlyingObjectSO cannonBallSO;

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool willDestroy = false;
    private float destroyTimer;
    private readonly float destroyDelay = 0.5f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.velocity = direction * cannonBallSO.speed;
        }
    }
    private void Update() {
        if (willDestroy) {
            destroyTimer += Time.deltaTime;
            if (destroyTimer >= destroyDelay) {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        OnCannonBallCollided?.Invoke(this, EventArgs.Empty);
        rb.velocity = Vector2.zero;
        willDestroy = true;
        destroyTimer = 0f;
    }

    public void SetDirection(Vector2 direction) {
        this.direction = direction.normalized;
    }

    public FlyingObjectSO GetFlyingObjectSO() {
        return cannonBallSO;
    }
}
