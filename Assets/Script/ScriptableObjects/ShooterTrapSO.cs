using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trap/ShooterTrap")]
public class ShooterTrapSO : ScriptableObject
{
    [Header("General")]
    public string trapName;
    public int scoreOnDead;
    public float maxHealth;
}