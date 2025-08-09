using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    None,
    MainMenuScene,
    LoadingScene,
    ChooseLevelScene,
    PalmTreeIslandScene,
    PirateShipScene,
    BossScene,
    SeaShoreScene,
    LargeCaveScene,
    CrabbyBossScene,
    TutorialScene,
    CreditsScene,
}

public static class SceneLoader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }

    public const Scene firstNormalLevelScene = Scene.PalmTreeIslandScene;
    public const Scene firstBossLevelScene = Scene.BossScene;

    public static event Action<Scene> OnGameSceneLoaded;
    public static event Action<Scene> OnNonGameSceneLoaded;

    private static Scene targetScene = Scene.MainMenuScene;
    private static Scene currentScene = Scene.None;
    private static Scene lastScene = Scene.MainMenuScene;

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    static SceneLoader()
    {
        SceneManager.sceneLoaded += OnSceneLoadedWrapper;
    }

    private static void OnSceneLoadedWrapper(UnityEngine.SceneManagement.Scene unityScene, LoadSceneMode mode)
    {
        if (unityScene.name == Scene.LoadingScene.ToString())
            return;

        if (Enum.TryParse(unityScene.name, out Scene parsed))
        {
            currentScene = parsed;
            if (IsGameScene(parsed))
            {
                OnGameSceneLoaded?.Invoke(parsed);
            }
            else
            {
                OnNonGameSceneLoaded?.Invoke(parsed);
            }
        }
        else
        {
            Debug.Log($"SceneLoader: Received unexpected scene name '{unityScene.name}'");
        }
    }

    public static void Load(Scene targetScene)
    {
        if (IsInDevelopment(targetScene))
        {
            DialogManager.Instance.ShowDialog(new() { 
                Title = "Coming soon!", 
                Message = "This scene is still in development. Please check back later.", 
                Buttons = { } 
            });
            return;
        }

        lastScene = SceneLoader.targetScene;
        SceneLoader.targetScene = targetScene;

        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(targetScene));
        };

        Debug.Log($"Loading scene: {targetScene}");
        SceneManager.LoadScene(Scene.LoadingScene.ToString()); // calling target scene here will immediately skip loading scene -> callback
    }

    private static IEnumerator LoadSceneAsync(Scene targetScene)
    {
        loadingAsyncOperation = SceneManager.LoadSceneAsync(targetScene.ToString());
        loadingAsyncOperation.allowSceneActivation = false;

        while (loadingAsyncOperation.progress < 0.9f)
            yield return null;

        loadingAsyncOperation.allowSceneActivation = true;
    }

    public static void LoadLastScene()
    {
        if (lastScene == Scene.LoadingScene)
        {
            // If we are already in the loading scene, we should not change the target scene
            // to avoid an infinite loop of loading the same scene.
            return;
        }

        Debug.Log($"Loading last scene: {lastScene}");
        targetScene = lastScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 0f; // Return 0 if no loading operation is in progress
        }
    }

    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            currentScene = targetScene;
            onLoaderCallback();
            Debug.Log($"LoaderCallback: {currentScene}");
        }
    }

    public static void ReloadCurrentScene()
    {
        Debug.Log($"Reloading current scene: {currentScene}");
        Load(currentScene);
    }

    public static bool IsGameScene(Scene scene)
    {
        return scene == Scene.PalmTreeIslandScene ||
               scene == Scene.PirateShipScene ||
               scene == Scene.BossScene ||
               scene == Scene.SeaShoreScene ||
               scene == Scene.LargeCaveScene ||
               scene == Scene.CrabbyBossScene ||
               scene == Scene.TutorialScene;
    }

    public static bool IsInDevelopment(Scene scene)
    {
        return scene == Scene.SeaShoreScene ||
               scene == Scene.LargeCaveScene ||
               scene == Scene.CrabbyBossScene;
    }
}
