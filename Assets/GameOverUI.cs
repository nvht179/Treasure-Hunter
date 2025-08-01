using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeTakenText;

    private void Awake()
    {
        homeButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });

        restartButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.ReloadCurrentScene();
        });

        exitButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(SceneLoader.Scene.ChooseLevelScene);
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

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        Debug.Log("GameOverUI -> GameManager_OnStateChanged");
        if (GameManager.Instance.IsGameOver())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
