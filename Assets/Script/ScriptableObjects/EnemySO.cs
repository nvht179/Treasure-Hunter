using UnityEngine;

namespace Script.ScriptableObjects
{
    [CreateAssetMenu]
    public class EnemySO : ScriptableObject
    {
        public string enemyName;
        public Transform prefab;
        public float maxHealth;
        public int scoreOnDead;
    }
}