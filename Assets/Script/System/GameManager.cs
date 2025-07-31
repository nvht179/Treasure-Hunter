using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentManager<GameManager>
{

    public event EventHandler OnStateChanged;

    private enum State
    {
        WaitingToStart,
        GamePlaying,
        Paused,
        LevelWon,
        LevelLost
    }

    [SerializeField] private State state;

    protected override void Awake()
    {
        base.Awake();

        SceneLoader.OnGameSceneLoaded += HandleGameSceneLoaded;
        SetState(State.WaitingToStart);

        Debug.Log("GameManager: Awake called, initializing game state.");
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResumeAction += GameInput_OnResumeAction;

        GamePauseManager.Instance.OnPauseRequested += () =>
        {
            if (state == State.GamePlaying)
                SetState(State.Paused);
        };
        GamePauseManager.Instance.OnResumeRequested += () =>
        {
            if (state == State.Paused)
                SetState(State.GamePlaying);
        };
        GamePauseManager.Instance.OnReturnToMainMenuRequested += () =>
        {
            SetState(State.WaitingToStart);
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        };

        Debug.Log("GameManager: Waiting for game scene to load...");
    }

    private void OnDestroy()
    {
        SceneLoader.OnGameSceneLoaded -= HandleGameSceneLoaded;

    }

    private void HandleGameSceneLoaded(SceneLoader.Scene loaded)
    {
        // Now the target scene has finished loading!
        var player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.Log("GameManager: No player found in scene. Cannot set up game state.");
            return;
        }

        Debug.Log("GameManager: Player found in scene, setting up game state.");
        player.OnPlayerDied += HandlePlayerDied;
        player.OnPlayerWon += HandlePlayerWon;
        SetState(State.GamePlaying);
    }

    private void HandlePlayerWon()
    {
        SetState(State.LevelWon);
    }

    private void HandlePlayerDied()
    {
        SetState(State.LevelLost);
    }

    private void GameInput_OnResumeAction(object sender, EventArgs e)
    {
        if(state == State.Paused)
        {
            Debug.Log("GameInput_OnPauseAction -> resume");
            GamePauseManager.Instance.ResumeGame();
            SetState(State.GamePlaying);
        } 
        else
        {
            Debug.Log("Not in a paused state, cannot resume.");
        }
        
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        if (state == State.GamePlaying)
        {
            Debug.Log("GameInput_OnPauseAction -> pause");
            GamePauseManager.Instance.PauseGame();
            SetState(State.Paused);
        }
        else
        {
            Debug.Log("Not in a playable state, cannot pause.");
        }
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

    private void SetState(State newState)
    {
        if (state == newState) return;
        state = newState;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"GameManager: State changed to {state}");
    }
}
