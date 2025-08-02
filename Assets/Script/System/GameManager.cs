using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentManager<GameManager>
{

    public event Action<State, State> OnStateChanged;

    public enum State
    {
        None,
        WaitingToStart,
        GamePlaying,
        Paused,
        LevelWon,
        LevelLost
    }

    [SerializeField] private State state;
    public State CurrentState => state;

    protected override void Awake()
    {
        base.Awake();

        SceneLoader.OnGameSceneLoaded += HandleGameSceneLoaded;
        SceneLoader.OnNonGameSceneLoaded += HandleNonGameSceneLoaded;

        state = State.None;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnResumeAction += GameInput_OnResumeAction;

        GamePauseManager.Instance.OnPauseRequested += () =>
        {
            if (state == State.GamePlaying)
            {
                SetState(State.Paused);
            }
        };
        GamePauseManager.Instance.OnResumeRequested += () =>
        {
            if (state == State.Paused)
            {
                SetState(State.GamePlaying);
            }
        };
        GamePauseManager.Instance.OnReturnToMainMenuRequested += () =>
        {
            SetState(State.WaitingToStart);
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        };
    }

    private void OnDestroy()
    {
        SceneLoader.OnGameSceneLoaded -= HandleGameSceneLoaded;
        SceneLoader.OnNonGameSceneLoaded -= HandleNonGameSceneLoaded;
    }

    private void HandleNonGameSceneLoaded(SceneLoader.Scene loadedScene)
    {
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.UI);
        SetState(State.WaitingToStart);
        Debug.Log("GameManager: Non-game scene found.");
    }

    private void HandleGameSceneLoaded(SceneLoader.Scene loadedScene)
    {
        // Now the target scene has finished loading!
        var player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.Log("GameManager: No player found in scene. Cannot set up game state.");
            GameInput.Instance.EnableActionMap(GameInput.ActionMap.UI);
            SetState(State.WaitingToStart);
            return;
        }

        Debug.Log("GameManager: Player found in GAME scene, setting up game state.");
        player.OnDead += HandlePlayerDied;
        player.OnWon += HandlePlayerWon;
        SetState(State.GamePlaying);

        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);

        SoundManager.Instance.AttachPlayerSound(player);
        var shop = FindObjectOfType<Shop>();
        var shopUI = FindObjectOfType<ShopUI>();
        if (shop != null && shopUI != null)
        {
            SoundManager.Instance.AttachShopSound(shop, shopUI);
        }
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
            SetState(State.Paused);
        }
        else
        {
            Debug.Log("Not in a playable state, cannot pause.");
        }
    }

    public int GetScore()
    {
        return 0;
    }

    public string GetTimeTaken()
    {
        return "hh:mm:ss";
    }

    private void SetState(State newState)
    {
        if (state == newState) return;
        State oldState = state;
        state = newState;
        OnStateChanged?.Invoke(oldState, newState);
        // Log the function inside OnStateChanged
        foreach (var action in OnStateChanged.GetInvocationList())
        {
            Debug.Log($"OnStateChanged action: {action.Method.Name}");
        }
        Debug.Log($"GameManager: State changed from {oldState} to {state}");
    }
}
