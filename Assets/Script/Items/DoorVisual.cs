using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorVisual : MonoBehaviour {
    private static readonly int Interacted = Animator.StringToHash("Interacted");

    [SerializeField] private Door door;
    [SerializeField] private Transform interactedVisual;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        door.OnDoorInteracted += Door_OnDoorInteracted;
    }

    private void Door_OnDoorInteracted(object sender, EventArgs e) {
        animator.SetTrigger(Interacted);
        interactedVisual.gameObject.SetActive(false);
    }
}
