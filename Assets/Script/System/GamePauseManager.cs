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
        settingsButton.onClick.AddListener(() =>
        {
            Hide();
            SettingsMenuUI.Instance.Show(Show);
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            OnReturnToMainMenuRequested?.Invoke();
        });

        Hide();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.UI);
        Show();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);
        Hide();
        OnResumeRequested?.Invoke();
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
