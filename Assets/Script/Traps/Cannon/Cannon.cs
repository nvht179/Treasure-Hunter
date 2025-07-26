using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {
    public enum FireDirection {
        Left,
        Right
    }

    public event EventHandler OnCannonBallFired;

    [SerializeField] private Transform firePoint;
    [SerializeField] private FlyingObjectSO cannonBallSO;
    [SerializeField] private float fireRate = 5f;
    [SerializeField] private FireDirection fireDirection;

    private float nextFireTime = 0f;
    private readonly float cannonBallDelay = 0.05f;

    private void Update() {
        if(Time.time >= nextFireTime) {
            FireCannon();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FireCannon() {
        OnCannonBallFired?.Invoke(this, EventArgs.Empty);
        StartCoroutine(DelayedFireCannonBall());
    }

    private IEnumerator DelayedFireCannonBall() {
        yield return new WaitForSeconds(cannonBallDelay);

        Transform cannonBallTransform = Instantiate(cannonBallSO.prefab, firePoint.position, firePoint.rotation);
        cannonBallTransform.SetParent(transform);
        CannonBall cannonBall = cannonBallTransform.GetComponent<CannonBall>();
        if (cannonBall != null) {
            Vector2 direction = fireDirection == FireDirection.Left ? Vector2.left : Vector2.right;
            cannonBall.SetDirection(direction);
        }
    }

    public FireDirection GetDirection()
    {
        return fireDirection;
    }
}