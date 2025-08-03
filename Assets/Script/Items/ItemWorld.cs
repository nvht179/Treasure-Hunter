using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour {
    public static ItemWorld SpawnItemWorld(Vector3 position, Item item, Transform prefab = null) {
        Transform itemTransform;
        if (prefab) {
            itemTransform = Instantiate(prefab, position, Quaternion.identity);
        }
        else {
            itemTransform = Instantiate(item.itemSO.prefab, position, Quaternion.identity);
        }

        // Find the foreground text component
        Transform foregroundTransform = itemTransform.Find("QuantityCanvas/number/foreground");
        if (foregroundTransform != null) {
            if (foregroundTransform.TryGetComponent<TMPro.TextMeshProUGUI>(out var textComponent)) {
                textComponent.text = item.quantity.ToString();
            }
        }

        // Add ItemWorld component if not present
        if (!itemTransform.TryGetComponent<ItemWorld>(out var itemWorld)) {
            itemWorld = itemTransform.gameObject.AddComponent<ItemWorld>();
        }

        itemWorld.SetItem(item);
        return itemWorld;
    }


    [SerializeField] private ItemSO itemSO;
    protected Item item;

    private void Awake() {
        item = new Item(itemSO, 1);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (item.itemSO.collectEffectPrefab != null)
        {
            var effect = Instantiate(
                item.itemSO.collectEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(effect.gameObject, 3f);
        }

        if (collision.TryGetComponent<Player>(out Player player)) {
            DestroySelf();
        }
    }

    public void SetItem(Item item) {
        this.item = item;
    }

    public Item GetItem() {
        return item;
    }

    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void DestroySelf(float delay) {
        Destroy(gameObject, delay);
    }
}
