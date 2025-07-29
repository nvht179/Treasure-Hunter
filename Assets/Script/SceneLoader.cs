using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        MainMenuScene = 0,
        LoadingScene = 1,
        GameScene = 2,
        OptionsScene = 3,
        HowToPlayScene = 4,
        CreditsScene = 5
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        SceneLoader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString()); // calling target scene here will immediately skip loading scene -> callback
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
