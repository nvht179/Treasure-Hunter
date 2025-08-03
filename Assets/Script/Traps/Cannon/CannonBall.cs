using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : FlyingObject {

    public event EventHandler OnCannonBallCollided;

    private void Awake() {
        DestroyDelay = 0.3f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((collisionLayer & (1 << collision.gameObject.layer)) != 0) {
            OnCannonBallCollided?.Invoke(this, EventArgs.Empty);

            var hitPlayer = collision.GetComponent<Player>();
            DamageObject(hitPlayer);

            StopFlying();
        }
    }
}