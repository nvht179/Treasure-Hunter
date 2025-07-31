using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentManager<GameManager>
{
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

    protected override void Awake()
    {
        base.Awake();
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResumeAction += GameInput_OnResumeAction;
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);
        //GamePauseManager.Instance.PauseGame();
    }

    private void GameInput_OnResumeAction(object sender, EventArgs e)
    {
        Debug.Log("GameInput_OnPauseAction -> resume");
        GamePauseManager.Instance.ResumeGame();
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        Debug.Log("GameInput_OnPauseAction -> pause");
        GamePauseManager.Instance.PauseGame();
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
