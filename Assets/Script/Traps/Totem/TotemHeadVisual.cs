using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHeadVisual : ShooterTrapVisual {
    private static readonly int Attack = Animator.StringToHash("Attack");

    private new void Awake() {
        base.Awake();
    }

    private void Start() {
        if (shooterTrap is TotemHead totemHead) {
            totemHead.OnObjectFired += head1A_OnObjectFired;
        }
    }

    private void head1A_OnObjectFired(object sender, EventArgs e) {
        animator.SetTrigger(Attack);
    }
}
