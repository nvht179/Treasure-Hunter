using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSpike : FlyingObject {

    private void Awake() {
        DestroyDelay = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((collisionLayer & (1 << collision.gameObject.layer)) != 0) {
            var hitPlayer = collision.GetComponent<Player>();
            DamageObject(hitPlayer);

            StopFlying();
        }
    }
}
