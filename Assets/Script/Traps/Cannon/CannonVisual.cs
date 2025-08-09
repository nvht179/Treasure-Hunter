using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonVisual : ShooterTrapVisual {
    private static readonly int Fire = Animator.StringToHash("Fire");

    [SerializeField] private GameObject fireEffect;

    private Coroutine effectCoroutine;
    private const float FireEffectDelay = 0.1f;
    private const float EffectDuration = 0.3f;

    private new void Awake() {
        base.Awake();
        if (shooterTrap.GetDirection() == ShooterTrap.FireDirection.Right) {
            fireEffect.transform.localPosition = new Vector3(-fireEffect.transform.localPosition.x,
                fireEffect.transform.localPosition.y, fireEffect.transform.localPosition.z);
        }
    }

    private new void Start() {
        base.Start();
        if (shooterTrap is Cannon cannon) {
            cannon.OnShoot += Cannon_OnCannonBallFired;
        }
    }

    private void Cannon_OnCannonBallFired(object sender, EventArgs e)
    {
        animator.SetTrigger(Fire);
        if (effectCoroutine != null) {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(PlayFireEffectWithDelay());
    }

    private IEnumerator PlayFireEffectWithDelay() {
        yield return new WaitForSeconds(FireEffectDelay);

        fireEffect.SetActive(true);
        yield return new WaitForSeconds(EffectDuration);
        fireEffect.SetActive(false);
    }
}