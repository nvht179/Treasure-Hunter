using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShooterTrap;

public class Cannon : ShooterTrap {
    private new void Awake() {
        base.Awake();
        fireDelay = 0.05f;
    }
}