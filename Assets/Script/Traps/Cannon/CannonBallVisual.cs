using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallVisual : MonoBehaviour {
    [SerializeField] private CannonBall cannonBall;
    [SerializeField] private GameObject explodeEffect;

    private void Start() {
        cannonBall.OnCannonBallCollided += CannonBall_OnCannonBallCollided;
    }

    private void CannonBall_OnCannonBallCollided(object sender, EventArgs e) {
        gameObject.SetActive(false);
        explodeEffect.SetActive(true);
    }
}
