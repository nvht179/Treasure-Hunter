using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour {

    public static CoinSpawner Instance { get; private set; }

    [SerializeField] private ItemSO coinSO;
    [SerializeField] private Transform coinWorldPrefab;

    private void Awake() {
        Instance = this;
    }

    public void SpawnCoin(Vector3 position) {
        int randomChance = Random.Range(0, 100);
        if (randomChance < 50) { // 50% chance
            int amount = Random.Range(1, 4);
            SpawnCoinItem(position, amount);
        } else if (randomChance < 70) { // 20% chance
            int amount = Random.Range(4, 7);
            SpawnCoinItem(position, amount);
        } else if (randomChance < 80) { // 20% chance
            int amount = Random.Range(7, 9);
            SpawnCoinItem(position, amount);
        } else {
            // No coin spawned
        }
    }

    private void SpawnCoinItem(Vector3 position, int amount) {
        ItemWorld.SpawnItemWorld(position, new Item(coinSO, amount), coinWorldPrefab);
    }


}
