using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeashellVisual : ShooterTrapVisual {
    private static readonly int Fire = Animator.StringToHash("Fire");
    private static readonly int Hit = Animator.StringToHash("Hit");

    private new void Awake() {
        base.Awake();
    }

    private void Start() {
        if (shooterTrap is Seashell seashell) {
            seashell.OnObjectFired += Seashell_OnObjectFired;
            seashell.OnDamageTaken += Seashell_OnDamageTaken;
            seashell.OnDestroyed += Seashell_OnDestroyed;
        }
    }

    private void Seashell_OnDestroyed(object sender, EventArgs e) {
       gameObject.SetActive(false);
    }

    private void Seashell_OnDamageTaken(object sender, IDamageable.OnDamageTakenEventArgs e) {
        animator.SetTrigger(Hit);
    }

    private void Seashell_OnObjectFired(object sender, EventArgs e) {
        animator.SetTrigger(Fire);
    }
}
