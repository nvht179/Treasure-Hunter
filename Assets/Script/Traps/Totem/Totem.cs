using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour {
    [SerializeField] private TotemConfigSO config;
    [SerializeField] private int minHeads = 1;
    [SerializeField] private int maxHeads = 3;
    [SerializeField] ShooterTrap.FireDirection fireDirection = ShooterTrap.FireDirection.Left;
    [SerializeField] private bool allowDups = false;

    private void Awake() {
        if(allowDups || minHeads > config.allHeads.Length) {
            BuildTotem2();
        } else {
            BuildTotem();
        }
    }

    public void BuildTotem() {
        // 1) clean up any previous heads
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 2) determine how many heads
        int count = Random.Range(minHeads, maxHeads + 1);

        // 3) shuffle & pick unique kinds
        var pool = new List<TotemHeadSO>(config.allHeads);
        for (int i = 0; i < pool.Count; i++) {
            int j = Random.Range(i, pool.Count);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        float currentY = 0;

        for (int i = 0; i < count; i++) {
            var headSO = pool[i];

            // choose variant: top if last in the stack
            bool isTop = (i == count - 1);
            Sprite sprite = isTop ? headSO.topSprite : headSO.normalSprite;
            Transform prefab = isTop ? headSO.topPrefab : headSO.normalPrefab;

            // instantiate & position
            var headT = Instantiate(prefab, transform);
            headT.localPosition = new Vector3(-headSO.deltaWidth * (fireDirection == ShooterTrap.FireDirection.Left ? 1 : -1), currentY, 0);
            
            // override sprite if needed
            if (headT.TryGetComponent<SpriteRenderer>(out var sr)) sr.sprite = sprite;

            // Set the shoot direction on the head
            if (headT.TryGetComponent<TotemHead>(out var totemHead))
                totemHead.SetFireDirection(fireDirection);

            // bump up for the next one
            currentY += headSO.height;
        }
    }

    public void BuildTotem2() {
        // 1) clean up any previous heads
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // 2) determine how many heads
        int count = Random.Range(minHeads, maxHeads + 1);

        // 3) allow duplicates & pick with replacement
        var pool = config.allHeads;

        float currentY = 0;

        for (int i = 0; i < count; i++) {
            // Pick a random head type from pool
            var headSO = pool[Random.Range(0, pool.Length)];

            // choose variant: top if last in the stack
            bool isTop = (i == count - 1);
            Sprite sprite = isTop ? headSO.topSprite : headSO.normalSprite;
            Transform prefab = isTop ? headSO.topPrefab : headSO.normalPrefab;

            // horizontal offset based on fire direction
            float xOffset = -headSO.deltaWidth * (fireDirection == ShooterTrap.FireDirection.Left ? 1 : -1);

            // instantiate & position
            var headT = Instantiate(prefab, transform);
            headT.localPosition = new Vector3(xOffset, currentY, 0);

            // override sprite if needed
            if (headT.TryGetComponent<SpriteRenderer>(out var sr)) sr.sprite = sprite;

            // set direction
            if (headT.TryGetComponent<TotemHead>(out var totemHead))
                totemHead.SetFireDirection(fireDirection);

            currentY += headSO.height;
        }
    }
}