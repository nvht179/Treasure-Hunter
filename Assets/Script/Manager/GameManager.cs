using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private enum State
    {
        WaitingToStart,
        //CountdownToStart,
        GamePlaying,
        Paused,
        LevelWon,
        LevelLost
    }

    private State state;
    private bool isGamePaused = false;

    private void Awake()
    {
        Instance = this; // TODO: need dont destroy on load ???
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResumeAction += GameInput_OnResumeAction;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);
        PauseManager.Instance.PauseGame();
    }

    private void GameInput_OnResumeAction(object sender, EventArgs e)
    {
        Debug.Log("GameInput_OnPauseAction -> resume");
        PauseManager.Instance.TogglePauseGame();
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        Debug.Log("GameInput_OnPauseAction -> pause");
        PauseManager.Instance.TogglePauseGame();
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsGameWon()
    {
        return state == State.LevelWon;
    }

    public bool IsGameOver()
    {
        return state == State.LevelLost;
    }
}
