using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    // TODO: Capitalize?
    public AudioClip[] move;
    public AudioClip[] jump;
    public AudioClip[] jumpLand;

    public AudioClip[] attack;
    public AudioClip[] airAttack;
    public AudioClip[] throwKnife;
    public AudioClip[] swordHit;
    public AudioClip[] swordMiss;
    public AudioClip[] playerGotHit;
    public AudioClip[] playerDead;

    public AudioClip[] inventoryOpen;
    public AudioClip[] inventoryClose;
    public AudioClip[] shopOpen;
    public AudioClip[] shopClose;
    public AudioClip doorOpenClose;

    [HideInInspector] 
    public AudioClip[] itemUse; // TODO: can be removed
    public AudioClip[] itemDrop;
    public AudioClip itemBuy;
    public AudioClip greenPotionSuccess;
    public AudioClip greenPotionFail;
    public AudioClip bluePotionUsed;
    public AudioClip redPotionUsed;
    public AudioClip keyCollected;

    public AudioClip buttonClick;
    public AudioClip won;
    public AudioClip lost;
}
