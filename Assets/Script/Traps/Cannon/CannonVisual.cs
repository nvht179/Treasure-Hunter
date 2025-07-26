using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonVisual : MonoBehaviour
{
    private static readonly int Fire = Animator.StringToHash("Fire");

    [SerializeField] private Cannon cannon;
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private Transform fireOrigin;

    private Animator animator;
    private Coroutine effectCoroutine;
    private const float FireEffectDelay = 0.1f;
    private const float EffectDuration = 0.3f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (cannon.GetDirection() == Cannon.FireDirection.Right)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            fireOrigin.localPosition = new Vector3(-fireOrigin.localPosition.x, fireOrigin.localPosition.y, fireOrigin.localPosition.z);
            fireEffect.transform.localPosition = new Vector3(-fireEffect.transform.localPosition.x,
                fireEffect.transform.localPosition.y, fireEffect.transform.localPosition.z);
        }
    }

    private void Start()
    {
        cannon.OnCannonBallFired += Cannon_OnCannonBallFired;
    }

    private void Cannon_OnCannonBallFired(object sender, EventArgs e)
    {
        animator.SetTrigger(Fire);
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(PlayFireEffectWithDelay());
    }

    private IEnumerator PlayFireEffectWithDelay()
    {
        yield return new WaitForSeconds(FireEffectDelay);

        fireEffect.SetActive(true);
        yield return new WaitForSeconds(EffectDuration);
        fireEffect.SetActive(false);
    }
}