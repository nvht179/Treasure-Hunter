using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyWorld : ItemWorld
{
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform effect;

    private void Awake() {
        item = new Item(itemSO, 1);
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.TryGetComponent<Player>(out Player player)) {
            visual.gameObject.SetActive(false);
            effect.gameObject.SetActive(true);
            DestroySelf(0.2f);
        }
    }
}
