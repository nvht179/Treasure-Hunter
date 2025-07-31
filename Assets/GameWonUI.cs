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
        GameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide(); // always hide at start, NOT awake
    }

    private void OnDestroy()
    {
        Debug.Log("GameWonUI -> OnDestroy");
        GameManager.Instance.OnStateChanged -= KitchenGameManager_OnStateChanged;
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        Debug.Log("GameWonUI -> KitchenGameManager_OnStateChanged");
        if (GameManager.Instance.IsGameWon())
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
