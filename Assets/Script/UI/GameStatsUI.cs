using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameStatsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI totalPlayTimeText;
    [SerializeField] private TextMeshProUGUI levelsCompletedText;
    [SerializeField] private TextMeshProUGUI completionPercentageText;
    [SerializeField] private TextMeshProUGUI collectiblesText;
    [SerializeField] private TextMeshProUGUI deathsText;
    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI lastPlayedText;
    
    [Header("UI Containers")]
    [SerializeField] private GameObject statsContainer;
    [SerializeField] private Button showStatsButton;
    [SerializeField] private Button hideStatsButton;
    [SerializeField] private Button refreshButton;
    
    [Header("Level Progress")]
    [SerializeField] private Transform levelProgressContainer;
    [SerializeField] private GameObject levelProgressItemPrefab;
    
    private void Start()
    {
        // Setup button listeners
        if (showStatsButton != null)
            showStatsButton.onClick.AddListener(ShowStats);
            
        if (hideStatsButton != null)
            hideStatsButton.onClick.AddListener(HideStats);
            
        if (refreshButton != null)
            refreshButton.onClick.AddListener(RefreshStats);
        
        // Initially hide stats
        HideStats();
        
        // Subscribe to DataManager events
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnGameProgressChanged += OnGameProgressChanged;
            DataManager.Instance.OnLevelCompleted += OnLevelCompleted;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (DataManager.Instance != null)
        {
            DataManager.Instance.OnGameProgressChanged -= OnGameProgressChanged;
            DataManager.Instance.OnLevelCompleted -= OnLevelCompleted;
        }
    }
    
    public void ShowStats()
    {
        RefreshStats();
        if (statsContainer != null)
            statsContainer.SetActive(true);
    }
    
    public void HideStats()
    {
        if (statsContainer != null)
            statsContainer.SetActive(false);
    }
    
    public void RefreshStats()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("GameStatsUI: DataManager not available");
            return;
        }
        
        var stats = DataManager.Instance.GetGameStatistics();
        UpdateStatsDisplay(stats);
        UpdateLevelProgress();
    }
    
    private void UpdateStatsDisplay(Dictionary<string, object> stats)
    {
        // Update text fields with stats
        if (totalScoreText != null && stats.ContainsKey("TotalScore"))
            totalScoreText.text = $"Total Score: {stats["TotalScore"]}";
            
        if (totalPlayTimeText != null && stats.ContainsKey("TotalPlayTime"))
            totalPlayTimeText.text = $"Play Time: {stats["TotalPlayTime"]}";
            
        if (levelsCompletedText != null && stats.ContainsKey("LevelsCompleted") && stats.ContainsKey("TotalLevels"))
            levelsCompletedText.text = $"Levels: {stats["LevelsCompleted"]}/{stats["TotalLevels"]}";
            
        if (completionPercentageText != null && stats.ContainsKey("CompletionPercentage"))
            completionPercentageText.text = $"Progress: {stats["CompletionPercentage"]:F1}%";
            
        if (collectiblesText != null && stats.ContainsKey("CollectiblesFound") && stats.ContainsKey("TotalCollectibles"))
            collectiblesText.text = $"Collectibles: {stats["CollectiblesFound"]}/{stats["TotalCollectibles"]} ({stats["CollectiblesPercentage"]:F1}%)";
            
        if (deathsText != null && stats.ContainsKey("TotalDeaths"))
            deathsText.text = $"Deaths: {stats["TotalDeaths"]}";
            
        if (winsText != null && stats.ContainsKey("TotalWins"))
            winsText.text = $"Wins: {stats["TotalWins"]}";
            
        if (lastPlayedText != null && stats.ContainsKey("LastPlayed"))
            lastPlayedText.text = $"Last Played: {stats["LastPlayed"]}";
    }
    
    private void UpdateLevelProgress()
    {
        if (levelProgressContainer == null || levelProgressItemPrefab == null)
            return;
            
        // Clear existing items
        foreach (Transform child in levelProgressContainer)
        {
            Destroy(child.gameObject);
        }
        
        if (DataManager.Instance?.GameProgress?.levelDataList == null)
            return;
        
        // Create progress items for each level
        foreach (var levelData in DataManager.Instance.GameProgress.levelDataList)
        {
            GameObject item = Instantiate(levelProgressItemPrefab, levelProgressContainer);
            var itemComponent = item.GetComponent<LevelProgressItem>();
            
            if (itemComponent != null)
            {
                itemComponent.SetupLevelData(levelData);
            }
            else
            {
                // Fallback - just update text if no custom component
                var text = item.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    string status = levelData.isCompleted ? "✓" : (levelData.isUnlocked ? "○" : "🔒");
                    text.text = $"{levelData.levelName} {status}";
                    
                    if (levelData.isCompleted)
                    {
                        text.text += $" - Best: {levelData.bestScore} ({levelData.GetFormattedBestTime()})";
                    }
                }
            }
        }
    }
    
    private void OnGameProgressChanged(GameProgressData progress)
    {
        // Auto-refresh if stats are currently shown
        if (statsContainer != null && statsContainer.activeInHierarchy)
        {
            RefreshStats();
        }
    }
    
    private void OnLevelCompleted(LevelData levelData)
    {
        // Auto-refresh if stats are currently shown
        if (statsContainer != null && statsContainer.activeInHierarchy)
        {
            RefreshStats();
        }
    }
}

// Optional component for custom level progress display
public class LevelProgressItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image backgroundImage;
    
    public void SetupLevelData(LevelData levelData)
    {
        if (levelNameText != null)
            levelNameText.text = levelData.levelName;
            
        if (statusText != null)
        {
            if (levelData.isCompleted)
                statusText.text = "✓ Completed";
            else if (levelData.isUnlocked)
                statusText.text = "○ Available";
            else
                statusText.text = "🔒 Locked";
        }
        
        if (scoreText != null)
        {
            if (levelData.isCompleted && levelData.bestScore > 0)
                scoreText.text = $"Best Score: {levelData.bestScore}";
            else
                scoreText.text = "No Score";
        }
        
        if (timeText != null)
        {
            if (levelData.isCompleted && levelData.bestTime < float.MaxValue)
                timeText.text = $"Best Time: {levelData.GetFormattedBestTime()}";
            else
                timeText.text = "No Time";
        }
        
        if (backgroundImage != null)
        {
            if (levelData.isCompleted)
                backgroundImage.color = new Color(0.2f, 0.8f, 0.2f, 0.3f); // Green tint
            else if (levelData.isUnlocked)
                backgroundImage.color = new Color(0.8f, 0.8f, 0.2f, 0.3f); // Yellow tint
            else
                backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Gray tint
        }
    }
}
