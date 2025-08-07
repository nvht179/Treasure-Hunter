using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour {

    private enum ResourceType {
        Coin,
        GreenGem,
        RedGem,
    }

    [SerializeField] private ResourceItemSO CoinSO;
    [SerializeField] private ResourceItemSO GreenGemSO;
    [SerializeField] private ResourceItemSO RedGemSO;
    [SerializeField] private ResourceItemSO GoldenKeySO;

    private Dictionary<ResourceType, ResourceItemSO> resourceItemSOMap;

    public static ResourceSpawner Instance { get; private set; }

    private void Awake() {
        Instance = this;
        resourceItemSOMap = new Dictionary<ResourceType, ResourceItemSO> {
            { ResourceType.Coin, CoinSO },
            { ResourceType.GreenGem, GreenGemSO },
            { ResourceType.RedGem, RedGemSO }
        };
    }

    public void SpawnMoney(Vector3 position, int minValue, int maxValue) {
        int total = Random.Range(minValue, maxValue);

        int redGemAmount = total / resourceItemSOMap[ResourceType.RedGem].value;
        total -= redGemAmount * resourceItemSOMap[ResourceType.RedGem].value;
        int greenGemAmount = total / resourceItemSOMap[ResourceType.GreenGem].value;
        total -= greenGemAmount * resourceItemSOMap[ResourceType.GreenGem].value;
        int coinAmount = total / resourceItemSOMap[ResourceType.Coin].value;
        total -= coinAmount * resourceItemSOMap[ResourceType.Coin].value;

        if (coinAmount > 0)
        {
            Vector3 coinPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.1f, 0.1f));
            ItemWorld.SpawnItemWorld(coinPos, new Item(CoinSO, coinAmount));
        }
        if (greenGemAmount > 0)
        {
            Vector3 greenGemPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.1f, 0.1f));
            ItemWorld.SpawnItemWorld(greenGemPos, new Item(GreenGemSO, greenGemAmount));
        }
        if (redGemAmount > 0)
        {
            Vector3 redGemPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.1f, 0.1f));
            ItemWorld.SpawnItemWorld(redGemPos, new Item(RedGemSO, redGemAmount));
        }
    }

    public void SpawnGoldenKey(Vector3 position) {
        ItemWorld.SpawnItemWorld(position, new Item(GoldenKeySO, 1));
    }
}
