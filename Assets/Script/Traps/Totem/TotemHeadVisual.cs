using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHeadVisual : ShooterTrapVisual {
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hit = Animator.StringToHash("Hit");

    private new void Awake() {
        base.Awake();
    }

    private new void Start() {
        base.Start();
        if (shooterTrap is TotemHead totemHead) {
            totemHead.OnShoot += TotemHead_OnObjectFired;
            totemHead.OnDamageTaken += TotemHead_OnDamageTaken;
            totemHead.OnDestroyed += TotemHead_OnDestroyed;
        }
    }

    private void TotemHead_OnDestroyed(object sender, EventArgs e) {
       gameObject.SetActive(false);
    }

    private void TotemHead_OnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e) {
        animator.SetTrigger(Hit);
    }

    private void TotemHead_OnObjectFired(object sender, EventArgs e) {
        animator.SetTrigger(Attack);
    }
}
