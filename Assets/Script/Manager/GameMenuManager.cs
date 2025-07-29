using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance { get; private set; }

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            PauseManager.Instance.ResumeGame();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        PauseManager.Instance.OnGamePaused += GameMenuManager_OnGamePaused;
        PauseManager.Instance.OnGameResumed += GameMenuManager_OnGameResumed;
    }

    private void GameMenuManager_OnGameResumed(object sender, EventArgs e)
    {
        Debug.Log("GameMenuManager_OnGameResumed");
        Hide();
    }

    private void GameMenuManager_OnGamePaused(object sender, EventArgs e)
    {
        Debug.Log("GameMenuManager_OnGamePaused");
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
