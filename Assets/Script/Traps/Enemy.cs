using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float enemyMoveSpeed = 2f;
    protected Player player;

    protected virtual void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    protected virtual void Update() {
        if (player == null) return;
        MoveToPlayer();
    }

    protected void MoveToPlayer()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.x = player.transform.position.x;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, enemyMoveSpeed * Time.deltaTime);
        HandleFlipX();
    }


    protected void HandleFlipX() {
        Vector2 directionToPlayer = player.transform.position - transform.position;
        if (directionToPlayer.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (directionToPlayer.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
