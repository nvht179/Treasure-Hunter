using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLevelUI : MonoBehaviour
{
    public enum LevelType
    {
        Normal, Boss
    }

    [System.Serializable]
    public struct LevelEntry
    {
        public SceneLoader.Scene scene;
    }

    [System.Serializable]
    public struct PageEntry
    {
        public List<LevelEntry> normalLevels;
        public LevelEntry bossLevel;
    }

    [Header("UI Prefabs")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform normalLevelContainer;
    [SerializeField] private Button bossLevelButton;

    [Header("Page-Level Data")]
    [SerializeField] private List<PageEntry> pageEntries;

    [Header("Other UI Elements")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private TextMeshProUGUI pageTitleText;

    private int currentPageIndex;

    private void Start()
    {
        currentPageIndex = 0;
        PopulateCurrentPageButtons();

        tutorialButton.onClick.AddListener(() =>
            SceneLoader.Load(SceneLoader.Scene.TutorialScene)
        );

        backButton.onClick.AddListener(() =>
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene)
        );

        nextPageButton.onClick.AddListener(() =>
        {
            currentPageIndex = (currentPageIndex + 1) % pageEntries.Count;
            PopulateCurrentPageButtons();
        });

        previousPageButton.onClick.AddListener(() =>
        {
            currentPageIndex = (currentPageIndex + pageEntries.Count - 1) % pageEntries.Count;
            PopulateCurrentPageButtons();
        });
    }

    private void PopulateCurrentPageButtons()
    {
        // Clear previous buttons   
        foreach (Transform child in normalLevelContainer)
        {
            Destroy(child.gameObject);
        }

        // TODO: need optimization
        int levelCount = 1;
        for (int i = 0; i < currentPageIndex; ++i)
        {
            levelCount += pageEntries[i].normalLevels.Count;
        }

        foreach (var entry in pageEntries[currentPageIndex].normalLevels)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, normalLevelContainer);
            var btn = buttonGO.GetComponent<Button>();
            var label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            label.text = levelCount.ToString();

            SceneLoader.Scene sceneToLoad = entry.scene; // capture safely
            btn.onClick.AddListener(() =>
            {
                SceneLoader.Load(sceneToLoad);
            });
            Debug.Log(btn.name + " will load scene: " + sceneToLoad.ToString());

            levelCount++;
        }

        // Change the listener for boss level button
        bossLevelButton.onClick.RemoveAllListeners();
        bossLevelButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(pageEntries[currentPageIndex].bossLevel.scene);
        });

        pageTitleText.text = (currentPageIndex + 1).ToString();
    }
}
