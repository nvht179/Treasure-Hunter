using System;
using UnityEngine;
using Script.Enemy.PinkStar;
using Script.Interfaces;

/// <summary>
/// A component that attaches sound effects to enemies and traps using manually assigned references.
/// </summary>
public class SoundAttacher : MonoBehaviour
{
    [SerializeField] private GameObject component;

    private IEnemy enemy;
    private IShooterTrap shooterTrap;

    private void Start()
    {
        if (component != null)
        {
            enemy = GetComponent<IEnemy>();
            shooterTrap = GetComponent<ShooterTrap>();
        }

        if (enemy != null)
        {
            SoundManager.Instance.AttachEnemySound(enemy);
        }
        else if (shooterTrap != null)
        {
            SoundManager.Instance.AttachShooterTrapSound(shooterTrap);
        }
        else
        {
            Debug.LogError("No suitable component found!");
        }
    }
}