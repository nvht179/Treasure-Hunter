using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeashellVisual : ShooterTrapVisual {
    private static readonly int Fire = Animator.StringToHash("Fire");

    private new void Awake() {
        base.Awake();
    }

    private void Start() {
        if (shooterTrap is Seashell seashell) {
            seashell.OnObjectFired += seashell_OnObjectFired;
        }
    }

    private void seashell_OnObjectFired(object sender, EventArgs e) {
        animator.SetTrigger(Fire);
    }
}
