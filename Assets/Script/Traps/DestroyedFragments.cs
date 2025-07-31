using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedFragments : MonoBehaviour {
    [SerializeField] private GameObject damageableGameObject;
    [SerializeField] private GameObject[] fragments;

    private IDamageable damageableObject;
    private bool isDestroyed = false;

    private void Awake() {
        damageableObject = damageableGameObject.GetComponent<IDamageable>();
        foreach (GameObject frag in fragments) {
            frag.SetActive(false);
        }
    }

    private void Update() {
        if(!isDestroyed) {
            return;
        }
        foreach (GameObject frag in fragments) {
            frag.SetActive(true);
            Rigidbody2D rb = frag.GetComponent<Rigidbody2D>();

            // Launch fragment in a random direction
            Vector2 force = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(3f, 6f);
            rb.AddForce(force, ForceMode2D.Impulse);
            rb.AddTorque(UnityEngine.Random.Range(-100f, 100f)); // Spin the fragment
        }
        isDestroyed = false;
    }

    private void Start() { 
        damageableObject.OnDestroyed += HandleDestroyed;
    }

    private void HandleDestroyed(object sender, EventArgs e) {
        isDestroyed = true;
    }
}
