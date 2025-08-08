using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseManager : PersistentManager<GamePauseManager>
{

    public event Action OnPauseRequested;
    public event Action OnResumeRequested;
    public event Action OnRestartRequested;
    public event Action OnReturnToMainMenuRequested;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Image blurBackground;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        resumeButton.onClick.AddListener(() =>
        {
            OnResumeRequested?.Invoke();
        });
        restartButton.onClick.AddListener(() =>
        {
            HideAll();
            SceneLoader.ReloadCurrentScene();
            OnRestartRequested?.Invoke();
        });
        settingsButton.onClick.AddListener(() =>
        {
            // no need to set time running again
            Hide();
            SettingsMenuUI.Instance.Show(Show);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            OnReturnToMainMenuRequested?.Invoke();
        });
        pauseButton.onClick.AddListener(() =>
        {
            OnPauseRequested?.Invoke();
        });

        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        GameManager_OnStateChanged(GameManager.State.None, GameManager.Instance.CurrentState);
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        if (newState == GameManager.State.GamePlaying)
        {
            Debug.Log("GamePauseManager: Resuming game, hiding pause menu and show pause button.");
            ResumeGame();
            Hide();
        }
        else if (newState == GameManager.State.Paused)
        {
            Debug.Log("GamePauseManager: Pausing game, showing pause menu and hiding pause button.");
            PauseGame();
            Show();
        }
        else
        {
            Debug.Log($"GamePauseManager: Game state changed to {newState}, hiding ALL pause menu.");
            HideAll();
            if (newState != GameManager.State.LevelWon && newState != GameManager.State.LevelLost)
            {
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    private void Show()
    {
        //gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        blurBackground.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
    }

    private void Hide()
    {
        //gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        blurBackground.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    private void HideAll()
    {
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        blurBackground.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
    }
}
