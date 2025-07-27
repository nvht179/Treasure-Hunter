using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pearl : FlyingObject {
    private void Awake() {
        destroyDelay = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if ((collisionLayer & (1 << collision.gameObject.layer)) != 0) {
            StopFlying();

            var hitPlayer = collision.GetComponent<Player>();
            DamageObject(hitPlayer);
        }
    }
}
