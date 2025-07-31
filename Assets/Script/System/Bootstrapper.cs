using UnityEngine;

public static class Bootstrapper
{
    private const string SystemResourcePath = "System";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        var systemPrefab = Resources.Load<GameObject>(SystemResourcePath);

        if (systemPrefab == null)
        {
            Debug.LogError("Bootstrapper: Failed to load 'System' prefab from Resources!");
            return;
        }

        var systemRoot = Object.Instantiate(systemPrefab);
        Object.DontDestroyOnLoad(systemRoot);
    }
}