using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterTrap : MonoBehaviour {

    public enum FireDirection {
        Left,
        Right
    }

    public event EventHandler OnObjectFired;

    [SerializeField] private Transform firePoint;
    [SerializeField] protected FlyingObjectSO flyingObjectSO;
    [SerializeField] private float fireRate;
    [SerializeField] private FireDirection fireDirection;

    private float nextFireTime = 0f;
    protected float fireDelay;

    private void Update() {
        if (Time.time >= nextFireTime) {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot() {
        OnObjectFired?.Invoke(this, EventArgs.Empty);
        StartCoroutine(DelayedFire());
    }

    private IEnumerator DelayedFire() {
        yield return new WaitForSeconds(fireDelay);

        Transform flyingObjectTransform = Instantiate(flyingObjectSO.prefab, firePoint.position, firePoint.rotation);
        flyingObjectTransform.SetParent(transform);
        if (flyingObjectTransform.TryGetComponent<FlyingObject>(out var flyingObject)) {
            Vector2 direction = fireDirection == FireDirection.Left ? Vector2.left : Vector2.right;
            flyingObject.SetDirection(direction);
        }
    }

    public FireDirection GetDirection() {
        return fireDirection;
    }

}
