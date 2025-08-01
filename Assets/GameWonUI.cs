using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button nextLevelButton;
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

        nextLevelButton.onClick.AddListener(() =>
        {
            Hide();
            //SceneLoader.ReloadCurrentScene();
        });

        exitButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(SceneLoader.Scene.ChooseLevelScene);
        });
    }

    private void Start()
    {
        Debug.Log("GameWonUI -> Start");
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide(); // always hide at start, NOT awake
    }

    private void OnDestroy()
    {
        Debug.Log("GameWonUI -> OnDestroy");
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        Debug.Log("GameWonUI -> GameManager_OnStateChanged");
        if (newState == GameManager.State.LevelWon)
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
        scoreText.text = $"Score: {GameManager.Instance.GetScore()}";
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
