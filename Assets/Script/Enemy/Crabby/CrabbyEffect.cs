using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabbyEffect : MonoBehaviour
{
    [SerializeField] private Crabby crabby;

    private static readonly int Attack = Animator.StringToHash("Attack");

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        crabby.OnAttack += CrabbyOnAttack;
    }

    private void CrabbyOnAttack(object sender, EventArgs e)
    {
        animator.SetTrigger(Attack);
    }
}