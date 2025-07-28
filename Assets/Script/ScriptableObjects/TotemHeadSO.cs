using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trap/TotemHead")]
public class TotemHeadSO : ScriptableObject {
    [Header("General")]
    public string totemName;
    public float height;
    public float deltaWidth; // to balance the width between heads

    [Header("Non-Top Variant")]
    public Sprite normalSprite;
    public Transform normalPrefab;

    [Header("Top Variant")]
    public Sprite topSprite;
    public Transform topPrefab;

}