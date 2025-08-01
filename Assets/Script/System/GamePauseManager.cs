using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseManager : PersistentManager<GamePauseManager>
{

    public event Action OnPauseRequested;
    public event Action OnResumeRequested;
    public event Action OnReturnToMainMenuRequested;

    [SerializeField] private Button resumeButton;
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
            ResumeGame();
            OnResumeRequested?.Invoke();
        });
        settingsButton.onClick.AddListener(() =>
        {
            Hide();
            pauseButton.gameObject.SetActive(false);
            SettingsMenuUI.Instance.Show(Show);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            ResumeGame();
            pauseButton.gameObject.SetActive(false);
            OnReturnToMainMenuRequested?.Invoke();
        });
        pauseButton.onClick.AddListener(() =>
        {
            PauseGame();
            OnPauseRequested?.Invoke();
        });

        Hide();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Show();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Hide();
    }

    private void Show()
    {
        //gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        blurBackground.gameObject.SetActive(true);

        pauseButton.gameObject.SetActive(false);
    }

    private void Hide()
    {
        //gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(false);
        blurBackground.gameObject.SetActive(false);

        pauseButton.gameObject.SetActive(true);
    }
}
