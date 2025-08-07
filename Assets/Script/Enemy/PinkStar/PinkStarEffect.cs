using System;
using UnityEngine;

namespace Script.Enemy.PinkStar
{
    public class PinkStarEffect : MonoBehaviour
    {
        [SerializeField] private PinkStarStateManager pinkStar;

        private static readonly int Attack = Animator.StringToHash("Attack");

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            pinkStar.OnCollisionEnter += PinkStarOnCollision;
        }

        private void PinkStarOnCollision(object sender, EventArgs e)
        {
            animator.SetTrigger(Attack);
        }
    }
}