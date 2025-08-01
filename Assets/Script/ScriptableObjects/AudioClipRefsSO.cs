using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip[] move;
    public AudioClip[] jump;
    public AudioClip[] jumpLand;
    public AudioClip[] attack;
    public AudioClip[] interact;
    public AudioClip[] inventoryOpen;
    public AudioClip[] inventoryClose;
    public AudioClip[] throwKnife;
    public AudioClip[] swordHit;
    public AudioClip[] swordMiss;
    public AudioClip[] opponentHit;
}
