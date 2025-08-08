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

    [SerializeField] private State state = State.None;
    public State CurrentState => state;
    
    // Game session data (not persisted)
    private int currentScore;
    private float levelLastPlayTimeStamp;
    private float levelPlayedTime;
    private int diamondsCollected;

    protected override void Awake()
    {
        base.Awake();

        SceneLoader.OnGameSceneLoaded += HandleGameSceneLoaded;
        SceneLoader.OnNonGameSceneLoaded += HandleNonGameSceneLoaded;

        SetState(State.WaitingToStart);
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
        GamePauseManager.Instance.OnRestartRequested += () =>
        {
            if (state == State.Paused)
            {
                SetState(State.WaitingToStart);
                SetState(State.GamePlaying);
            }
        };
        GamePauseManager.Instance.OnReturnToMainMenuRequested += () =>
        {
            SetState(State.WaitingToStart);
            SceneLoader.Load(Scene.MainMenuScene);
        };
    }

    private void OnDestroy()
    {
        SceneLoader.OnGameSceneLoaded -= HandleGameSceneLoaded;
        SceneLoader.OnNonGameSceneLoaded -= HandleNonGameSceneLoaded;
    }

    private void HandleNonGameSceneLoaded(Scene loadedScene)
    {
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.UI);
        SetState(State.WaitingToStart);
        Debug.Log("GameManager: Non-game scene found.");
    }

    private void HandleGameSceneLoaded(Scene loadedScene)
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

        diamondsCollected = 0;
        player.OnDead += HandlePlayerDead;
        player.OnDiamondCollected += (_, __) => diamondsCollected++;
        
        // Initialize game session
        currentScore = 0;
        levelLastPlayTimeStamp = Time.time;
        
        DataManager.Instance.StartLevel(loadedScene);
        SetState(State.GamePlaying);
        GameInput.Instance.EnableActionMap(GameInput.ActionMap.Player);
        SoundManager.Instance.AttachPlayerSound(player);

        var shop = FindObjectOfType<Shop>();
        var shopUI = FindObjectOfType<ShopUI>();
        if (shop != null && shopUI != null)
        {
            SoundManager.Instance.AttachShopSound(shop, shopUI);
        }

        var door = FindObjectOfType<Door>();
        if (door != null)
        {
            SoundManager.Instance.AttachDoorSound(door);
            door.OnDoorInteracted += (_, __) =>
            {
                HandlePlayerWon();
            };
        }
    }

    private void HandlePlayerWon()
    {
        SetState(State.LevelWon);
        DataManager.Instance.CompleteCurrentLevel(currentScore, levelPlayedTime, diamondsCollected);
        diamondsCollected = 0;
    }

    private void HandlePlayerDead(object sender, EventArgs e)
    {
        SetState(State.LevelLost);
        levelPlayedTime = Time.time - levelLastPlayTimeStamp;
        DataManager.Instance.RecordPlayerDeath();
    }

    private void GameInput_OnResumeAction(object sender, EventArgs e)
    {
        if(state == State.Paused)
        {
            SetState(State.GamePlaying);
            levelLastPlayTimeStamp = Time.time;
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
            levelPlayedTime += Time.time - levelLastPlayTimeStamp;
        }
        else
        {
            Debug.Log("Not in a playable state, cannot pause.");
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public string GetTimeTaken()
    {
        levelPlayedTime = Time.time - levelLastPlayTimeStamp;
        TimeSpan timeSpan = TimeSpan.FromSeconds(levelPlayedTime);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        Debug.Log($"GameManager: Score updated to {currentScore}");
    }

    private void SetState(State newState)
    {
        if (state == newState) return;
        State oldState = state;
        state = newState;
        OnStateChanged?.Invoke(oldState, newState);
        Debug.Log($"GameManager: State changed from {oldState} to {state}");
    }

    public int GetNumberOfDiamonds()
    {
        return diamondsCollected;
    }
}
