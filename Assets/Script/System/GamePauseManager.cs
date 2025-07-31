using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseManager : PersistentManager<GamePauseManager>
{

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;

    public bool isGamePaused;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        isGamePaused = false;

        resumeButton.onClick.AddListener(() =>
        {
            ResumeGame();
        });
        settingsButton.onClick.AddListener(() =>
        {
            Hide();
            SettingsMenuUI.Instance.Show(Show);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            isGamePaused = false;
            Hide();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });

        Hide();
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.UI);
        Show();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);
        Hide();
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
