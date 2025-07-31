using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopChestVisual : MonoBehaviour {
    private static readonly int OnOpen = Animator.StringToHash("OnOpen");

    [SerializeField] private Shop shop;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        shop.OnInteract += ShopOnInteract;
    }

    private void ShopOnInteract(object sender, EventArgs e) {
        animator.SetTrigger(OnOpen);
    }
}
