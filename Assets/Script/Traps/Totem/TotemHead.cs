using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemHead : ShooterTrap {
    
    private new void Awake() {
        base.Awake();
        fireDelay = 0.15f;
    }
}
