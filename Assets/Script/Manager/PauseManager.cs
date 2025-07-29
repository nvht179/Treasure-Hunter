using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResumed;

    public bool isGamePaused;

    private void Awake()
    {
        Instance = this;
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
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);
        OnGameResumed?.Invoke(this, EventArgs.Empty);
    }
}
