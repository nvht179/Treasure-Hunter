using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        MainMenuScene,
        LoadingScene,
        ChooseLevelScene,
        GameScene,
        PalmTreeIslandScene,
        PirateShipScene,
        BossScene,
        TutorialScene,
        CreditsScene,
    }

    public const Scene firstNormalLevelScene = Scene.PalmTreeIslandScene;
    public const Scene firstBossLevelScene = Scene.BossScene;

    public static event Action<Scene> OnGameSceneLoaded;
    public static event Action<Scene> OnNonGameSceneLoaded;

    private static Scene targetScene = Scene.MainMenuScene;
    private static Scene currentScene = Scene.GameScene;
    private static Scene lastScene = Scene.MainMenuScene;

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
            Debug.Log($"OnSceneLoadedWrapper: {parsed}");
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
        lastScene = SceneLoader.targetScene;
        SceneLoader.targetScene = targetScene;

        Debug.Log($"Loading scene: {targetScene}");
        SceneManager.LoadScene(Scene.LoadingScene.ToString()); // calling target scene here will immediately skip loading scene -> callback
    }

    public static void LoadLastScene()
    {
        if (lastScene == Scene.LoadingScene)
        {
            // If we are already in the loading scene, we should not change the target scene
            // to avoid an infinite loop of loading the same scene.
            return;
        }

        Debug.Log($"Loading last scene: {lastScene.ToString()}");
        targetScene = lastScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
        currentScene = targetScene;

        Debug.Log($"LoaderCallback: {currentScene}");
    }

    public static void ReloadCurrentScene()
    {
        Debug.Log($"Reloading current scene: {currentScene.ToString()}");
        Load(currentScene);
    }

    public static bool IsGameScene(Scene scene)
    {
        return scene == Scene.GameScene ||
               scene == Scene.PalmTreeIslandScene ||
               scene == Scene.PirateShipScene ||
               scene == Scene.BossScene ||
               scene == Scene.TutorialScene;
    }
}
