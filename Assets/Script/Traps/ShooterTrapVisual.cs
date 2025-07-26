using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterTrapVisual : MonoBehaviour {
    [SerializeField] protected ShooterTrap shooterTrap;
    [SerializeField] private Transform fireOrigin;

    protected Animator animator;

    protected void Awake() {
        animator = GetComponent<Animator>();
        if (shooterTrap.GetDirection() == ShooterTrap.FireDirection.Right)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            fireOrigin.localPosition = new Vector3(-fireOrigin.localPosition.x, fireOrigin.localPosition.y, fireOrigin.localPosition.z);
            
        }
    }
}
