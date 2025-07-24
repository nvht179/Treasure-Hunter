using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonVisual : MonoBehaviour {

    private const string FIRE = "Fire";

    [SerializeField] private Cannon cannon;
    [SerializeField] private GameObject fireEffect;

    private Animator animator;
    private Coroutine effectCoroutine;
    private readonly float fireEffectDelay = 0.1f;
    private readonly float effectDuration = 0.3f;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        cannon.OnCannonBallFired += Cannon_OnCannonBallFired;
    }

    private void Cannon_OnCannonBallFired(object sender, EventArgs e) {
        animator.SetTrigger(FIRE);
        if (effectCoroutine != null) {
            StopCoroutine(effectCoroutine);
        }
        effectCoroutine = StartCoroutine(PlayFireEffectWithDelay());
    }

    private IEnumerator PlayFireEffectWithDelay() {
        yield return new WaitForSeconds(fireEffectDelay);

        fireEffect.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        fireEffect.SetActive(false);
    }
}