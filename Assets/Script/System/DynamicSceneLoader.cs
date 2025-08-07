using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// NOTE: This class is currently disabled in favor of using the original SceneLoader
// for simplicity. It can be re-enabled if dynamic scene loading is needed in the future.
[System.Obsolete("Use SceneLoader instead for simpler, more reliable scene loading")]
public class DynamicSceneLoader : MonoBehaviour
{
    public static DynamicSceneLoader Instance { get; private set; }
    
    private void Awake()
    {
        Debug.LogWarning("DynamicSceneLoader is obsolete. Use SceneLoader instead.");
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    // Keeping the interface for backward compatibility but redirecting to SceneLoader
    public void LoadLevelScene(string levelId, Action<bool> onComplete = null)
    {
        Debug.LogWarning($"DynamicSceneLoader.LoadLevelScene is obsolete. Use SceneLoader instead for level: {levelId}");
        onComplete?.Invoke(false);
    }
}
