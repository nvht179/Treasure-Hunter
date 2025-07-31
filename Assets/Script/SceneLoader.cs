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
        HowToPlayScene,
        CreditsScene,
    }

    private static Scene targetScene = Scene.MainMenuScene;
    private static Scene lastScene = Scene.MainMenuScene;
    public const Scene firstNormalLevelScene = Scene.PalmTreeIslandScene;
    public const Scene firstBossLevelScene = Scene.BossScene;

    public static void Load(Scene targetScene)
    {
        lastScene = SceneLoader.targetScene;
        SceneLoader.targetScene = targetScene;

        Debug.Log($"Loading scene: {targetScene.ToString()}");
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
    }
}
