using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameLostUI : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private Transform visuals;

    private void Awake()
    {
        homeButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(Scene.MainMenuScene);
        });

        restartButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.ReloadCurrentScene();
        });

        exitButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(Scene.ChooseLevelScene);
        });
    }

    private void Start()
    {
        Debug.Log("GameOverUI -> Start");
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void OnDestroy()
    {
        Debug.Log("GameOverUI -> OnDestroy");
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        Debug.Log("GameOverUI -> GameManager_OnStateChanged");
        if (newState == GameManager.State.LevelLost)
        {
            StartCoroutine(ShowWithDelay());
        }
        else
        {
            StopAllCoroutines();
            Hide();
        }
    }

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Show();
    }

    public void Show()
    {
        scoreText.text = $"Score: {GameManager.Instance.GetScore()}";
        timeTakenText.text = $"Time: {GameManager.Instance.GetTimeTaken()}";
        visuals.gameObject.SetActive(true);
    }

    public void Hide()
    {
        visuals.gameObject.SetActive(false);
    }
}
