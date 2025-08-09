using System;
using System.Collections;
using System.Collections.Generic;
using Script.Interfaces;
using UnityEngine;

public class ShooterTrap : MonoBehaviour, IShooterTrap {

    public enum FireDirection {
        Left,
        Right
    }

    public event EventHandler<IShooterTrap.OnShootEventArgs> OnShoot;

    [SerializeField] private Transform firePoint;
    [SerializeField] protected FlyingObjectSO flyingObjectSO;
    [SerializeField] private float fireRate;
    [SerializeField] public FireDirection fireDirection;

    private float nextFireTime;
    private bool stopShooting;
    protected float FireDelay;

    protected virtual void Awake() {
        nextFireTime = UnityEngine.Random.Range(0f, fireRate);
    }

    private void Update() {
        if (stopShooting) return;
        if (Time.time >= nextFireTime) {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot() {
        OnShoot?.Invoke(this, new IShooterTrap.OnShootEventArgs
        {
            ShooterTrap = this
        });
        StartCoroutine(DelayedFire());
    }

    private IEnumerator DelayedFire() {
        yield return new WaitForSeconds(FireDelay);

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

    public void SetFireDirection(FireDirection direction) {
        fireDirection = direction;
    }

    protected void StopShooting() {
        stopShooting = true;
    }

}
