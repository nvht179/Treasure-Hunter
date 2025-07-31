using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneLoader.Scene.GameScene);
        });
        settingsButton.onClick.AddListener(() =>
        {
            SettingsMenuUI.Instance.Show(() => { });
        });
        quitButton.onClick.AddListener(() =>
        {
            Debug.Log("Quit button clicked");
            Application.Quit();
        });
    }
}
