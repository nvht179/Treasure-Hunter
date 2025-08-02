using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FlyingObjectSO : ScriptableObject {
    public string objectName;
    public float speed;
    public float damage;
    public float force;
    public Transform prefab;
    public Sprite sprite;
}
