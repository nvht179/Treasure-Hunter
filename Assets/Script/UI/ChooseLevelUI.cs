using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLevelUI : MonoBehaviour
{
    [Header("UI Prefabs")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform normalLevelContainer;
    [SerializeField] private Button bossLevelButton;

    [Header("Other UI Elements")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private TextMeshProUGUI pageNumberText;
    
    [Header("Visual Elements")]
    [HideInInspector]
    [SerializeField] private TextMeshProUGUI pageNameText;

    private int currentPageIndex;
    private int totalPages;

    private void Start()
    {        
        currentPageIndex = 0;
        totalPages = DataManager.Instance.GetNumberOfPages();
        
        // Load current page from DataManager if available
        if (DataManager.Instance.GameProgress != null)
        {
            currentPageIndex = Mathf.Clamp(DataManager.Instance.GameProgress.currentPage, 0, totalPages - 1);
        }
        
        PopulateCurrentPageButtons();

        tutorialButton.onClick.AddListener(() => {
            SceneLoader.Load(Scene.TutorialScene);
        });

        backButton.onClick.AddListener(() =>
            SceneLoader.Load(Scene.MainMenuScene)
        );

        nextPageButton.onClick.AddListener(() =>
        {
            currentPageIndex = (currentPageIndex + 1) % totalPages;
            SaveCurrentPage();
            PopulateCurrentPageButtons();
        });

        previousPageButton.onClick.AddListener(() =>
        {
            currentPageIndex = (currentPageIndex + totalPages - 1) % totalPages;
            SaveCurrentPage();
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

        if (DataManager.Instance != null)
        {
            PopulateFromConfig();
        }
        else
        {
            Debug.LogError($"ChooseLevelUI: No valid page data for index {currentPageIndex}");
        }
    }
    
    private void PopulateFromConfig()
    {
        var currentPage = DataManager.Instance.GetCurrentPage(currentPageIndex);
        
        // Update page visual elements
        if (pageNumberText != null)
            pageNumberText.text = (currentPageIndex + 1).ToString();
        
        // Populate normal levels
        foreach (var levelInfo in currentPage.NormalLevels)
        {
            CreateNormalLevelButton(levelInfo);
        }
        
        // Setup boss level
        if (currentPage.BossLevel != null)
        {
            SetupBossLevelButton(currentPage.BossLevel);
        }
        else
        {
            // Hide boss button if no boss level
            if (bossLevelButton != null)
                bossLevelButton.interactable = false;
        }
    }
    
    private void CreateNormalLevelButton(LevelPageConfigSO.LevelInfo levelInfo)
    {
        GameObject buttonGO = Instantiate(levelButtonPrefab, normalLevelContainer);
        var btn = buttonGO.GetComponent<Button>();
        var label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
        var btnImage = btn.GetComponent<Image>();

        // Set level number
        label.text = levelInfo.levelNumber.ToString();
        
        // Get level data from DataManager
        bool isUnlocked = levelInfo.unlockedByDefault;
        bool isCompleted = false;
        
        if (DataManager.Instance != null && DataManager.Instance.GameProgress != null)
        {
            var levelData = DataManager.Instance.GameProgress.GetLevelData(levelInfo.levelId);
            if (levelData != null)
            {
                isUnlocked = levelData.isUnlocked;
                isCompleted = levelData.isCompleted;
            }
        }
        
        // Visual feedback for level state
        if (!isUnlocked)
        {
            btn.interactable = false;
        }
        else if (isCompleted)
        {
            btn.interactable = true;
            btnImage.color = Color.yellow;
        }
        else
        {
            btn.interactable = true;
        }

        // Setup button click
        btn.onClick.AddListener(() =>
        {
            SceneLoader.Load(levelInfo.levelId);
        });
        
        Debug.Log($"Created level button: {levelInfo.displayName} (ID: {levelInfo.levelId}, Unlocked: {isUnlocked}, Completed: {isCompleted})");
    }
    
    private void SetupBossLevelButton(LevelPageConfigSO.LevelInfo bossInfo)
    {
        if (bossLevelButton == null) return;
        
        bossLevelButton.gameObject.SetActive(true);
        
        // Get boss data from DataManager
        bool isUnlocked = false;
        bool isCompleted = false;
        
        if (DataManager.Instance != null && DataManager.Instance.GameProgress != null)
        {
            var levelData = DataManager.Instance.GameProgress.GetLevelData(bossInfo.levelId);
            if (levelData != null)
            {
                isUnlocked = levelData.isUnlocked;
                isCompleted = levelData.isCompleted;
            }
        }
        
        // Visual feedback for boss level
        var bossImage = bossLevelButton.GetComponent<Image>();
        if (!isUnlocked)
        {
            bossImage.color = Color.gray;
            bossLevelButton.interactable = false;
        }
        else if (isCompleted)
        {
            bossImage.color = Color.yellow;
        }
        else
        {
            bossLevelButton.interactable = true;
        }
        
        // Setup button click
        bossLevelButton.onClick.RemoveAllListeners();
        bossLevelButton.onClick.AddListener(() =>
        {
            SceneLoader.Load(bossInfo.levelId);
        });
        
        Debug.Log($"Setup boss button: {bossInfo.displayName} (ID: {bossInfo.levelId}, Unlocked: {isUnlocked}, Completed: {isCompleted})");
    }
    
    private void SaveCurrentPage()
    {
        if (DataManager.Instance != null && DataManager.Instance.GameProgress != null)
        {
            DataManager.Instance.GameProgress.currentPage = currentPageIndex;
        }
    }
}
