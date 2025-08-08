using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWonUI : MonoBehaviour
{
    [SerializeField] private Transform visuals;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeTakenText;
    [SerializeField] private List<Image> diamondImages;

    private void Awake()
    {
        homeButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(Scene.MainMenuScene);
        });

        nextLevelButton.onClick.AddListener(() =>
        {
            Hide();
            Debug.Log("GameWonUI -> Next Level Button Clicked");
            SceneLoader.Load(DataManager.Instance.GetNextLevelID());
        });

        exitButton.onClick.AddListener(() =>
        {
            Hide();
            SceneLoader.Load(Scene.ChooseLevelScene);
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
        int numberOfDiamonds = GameManager.Instance.GetNumberOfDiamonds();

        Debug.Log("Number of Diamonds: " + numberOfDiamonds);

        for (int i = 0; i < Math.Min(numberOfDiamonds, 3); ++i)
        {
            diamondImages[i].color = Color.white;
        }
        visuals.gameObject.SetActive(true);
    }

    public void Hide()
    {
        visuals.gameObject.SetActive(false);
    }
}
