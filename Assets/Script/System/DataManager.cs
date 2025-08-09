using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class DataManager : PersistentManager<DataManager>
{
    [Header("Configuration")]
    [SerializeField] private DataManagerConfigSO config;
    [SerializeField] private LevelPageConfigSO levelPageConfig;
    [SerializeField] private ItemListSO defaultListSO;
    private bool useJsonSaveFormat = true; // fallback if no config
    private bool enableAutoSave = false; // fallback if no config
    private float autoSaveInterval = 30f; // fallback if no config
    
    // Events
    public event Action<UserPreferencesData> OnUserPreferencesChanged;
    public event Action<GameProgressData> OnGameProgressChanged;
    public event Action<LevelData> OnLevelCompleted;
    public event Action OnDataLoaded;
    public event Action OnDataSaved;
    
    // Data containers
    private SaveGameData currentSaveData;
    public UserPreferencesData UserPreferences => currentSaveData?.userPreferences;
    public GameProgressData GameProgress => currentSaveData?.gameProgress;
    
    // Constants
    private const string SAVE_FILE_NAME = "TreasureHunterSave.json";
    private const string PLAYER_PREFS_SAVE_KEY = "TreasureHunterSave";
    private const string BACKUP_SAVE_SUFFIX = "_backup";
    
    // Auto-save timer
    private float autoSaveTimer;
    private bool hasUnsavedChanges;
    public List<Item> CurrentPlayerInventoryItems;

    #region Unity base methods

    protected override void Awake()
    {
        base.Awake();
        
        // Apply configuration if available
        if (config != null)
        {
            useJsonSaveFormat = config.useJsonSaveFormat;
            enableAutoSave = config.enableAutoSave;
            autoSaveInterval = config.autoSaveInterval;
        }

        InitializeData();
        LoadGameData();
    }
    
    private void Start()
    {
        InitializeLevelData(); // Initialize level data if not exists

        // Auto-save setup
        autoSaveTimer = autoSaveInterval;
        
        Debug.Log("DataManager: Initialized successfully");
    }

    private void Update()
    {
        // Auto-save if having unsaved changes
        if (enableAutoSave && hasUnsavedChanges)
        {
            autoSaveTimer -= Time.unscaledDeltaTime;
            if (autoSaveTimer <= 0f)
            {
                SaveGameData();
                autoSaveTimer = autoSaveInterval;
            }
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && hasUnsavedChanges)
        {
            SaveGameData();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && hasUnsavedChanges)
        {
            SaveGameData();
        }
    }
    
    private void OnDestroy()
    {
        if (hasUnsavedChanges)
        {
            SaveGameData();
        }
    }

    #endregion

    #region Data Initialization

    private void InitializeData()
    {
        if (currentSaveData == null)
        {
            currentSaveData = new();
            hasUnsavedChanges = true;
        }
    }
    
    private void InitializeLevelData()
    {
        List<(Scene id, string name, bool isBoss, bool unlocked, int collectibles)> levelDefinitions = new()
        {
            (Scene.TutorialScene, "Tutorial Scene", false, true, 0)
        };
            
        foreach (var page in levelPageConfig.levelPages)
        {
            // Add normal levels
            foreach (var level in page.NormalLevels)
            {
                levelDefinitions.Add((
                    level.levelId,
                    level.displayName,
                    false, // not boss
                    level.unlockedByDefault || levelPageConfig.unlockAllLevel,
                    level.totalCollectibles
                ));
            }
                
            // Add boss level
            if (page.BossLevel != null)
            {
                levelDefinitions.Add((
                    page.BossLevel.levelId,
                    page.BossLevel.displayName,
                    true, // is boss
                    page.BossLevel.unlockedByDefault || levelPageConfig.unlockAllLevel,
                    page.BossLevel.totalCollectibles
                ));
            }
        }

        // Auto-unlock first level if configured
        if (levelPageConfig.autoUnlockFirstLevel && levelDefinitions.Count > 1)
        {
            var firstLevel = levelDefinitions[1];
            levelDefinitions[1] = (firstLevel.id, firstLevel.name, firstLevel.isBoss, true, firstLevel.collectibles);
        }
        
        foreach (var (id, name, isBoss, unlocked, collectibles) in levelDefinitions)
        {
            if (currentSaveData.gameProgress.GetLevelData(id) == null)
            {
                var levelData = new LevelData(id, name, isBoss)
                {
                    isUnlocked = unlocked,
                    totalCollectibles = collectibles
                };
                
                currentSaveData.gameProgress.levelDataList.Add(levelData);
                currentSaveData.gameProgress.totalLevels++;
                currentSaveData.gameProgress.totalCollectibles += collectibles;

                Debug.Log($"New level found, loading into currentSaveData: {id}");
            }
        }
        
        hasUnsavedChanges = true;
    }
    
    #endregion
    
    #region Save/Load Operations
    
    public bool SaveGameData()
    {
        try
        {
            currentSaveData.UpdateSaveDate();
            
            if (useJsonSaveFormat)
            {
                return SaveToJsonFile();
            }
            else
            {
                return SaveToPlayerPrefs();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: Failed to save game data - {e.Message}");
            return false;
        }
    }
    
    public bool LoadGameData()
    {
        try
        {
            bool loadSuccess = false;
            
            if (useJsonSaveFormat)
            {
                loadSuccess = LoadFromJsonFile();
            }
            else
            {
                loadSuccess = LoadFromPlayerPrefs();
            }
            
            if (loadSuccess)
            {
                OnDataLoaded?.Invoke();
                Debug.Log("DataManager: Game data loaded successfully");
            }
            
            return loadSuccess;
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: Failed to load game data - {e.Message}");
            return false;
        }
    }
    
    private bool SaveToJsonFile()
    {
        try
        {
            string savePath = GetSaveFilePath();
            string backupPath = GetSaveFilePath() + BACKUP_SAVE_SUFFIX;
            
            // Create backup of existing save
            if (File.Exists(savePath))
            {
                File.Copy(savePath, backupPath, true);
            }
            
            string jsonData = JsonUtility.ToJson(currentSaveData, true);
            File.WriteAllText(savePath, jsonData);
            
            hasUnsavedChanges = false;
            OnDataSaved?.Invoke();
            
            Debug.Log($"DataManager: Data saved to {savePath}");    
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: JSON save failed - {e.Message}");
            return false;
        }
    }
    
    private bool LoadFromJsonFile()
    {
        string savePath = GetSaveFilePath();

        if (!File.Exists(savePath))
        {
            Debug.Log("DataManager: No save file found, creating new data");
            return false;
        }
        
        try
        {
            string jsonData = File.ReadAllText(savePath);
            currentSaveData = JsonUtility.FromJson<SaveGameData>(jsonData);

            if (currentSaveData == null)
            {
                throw new Exception("Failed to deserialize save data");
            }
            
            hasUnsavedChanges = false;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: JSON load failed - {e.Message}");
            
            // Try to load backup
            string backupPath = GetSaveFilePath() + BACKUP_SAVE_SUFFIX;
            if (File.Exists(backupPath))
            {
                try
                {
                    string backupData = File.ReadAllText(backupPath);
                    currentSaveData = JsonUtility.FromJson<SaveGameData>(backupData);
                    Debug.Log("DataManager: Loaded from backup file");
                    return true;
                }
                catch
                {
                    Debug.LogError("DataManager: Backup file also corrupted");
                }
            }
            
            return false;
        }
    }
    
    private bool SaveToPlayerPrefs()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(currentSaveData);
            PlayerPrefs.SetString(PLAYER_PREFS_SAVE_KEY, jsonData);
            PlayerPrefs.Save();
            
            hasUnsavedChanges = false;
            OnDataSaved?.Invoke();
            
            Debug.Log("DataManager: Data saved to PlayerPrefs");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: PlayerPrefs save failed - {e.Message}");
            return false;
        }
    }
    
    private bool LoadFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PLAYER_PREFS_SAVE_KEY))
        {
            Debug.Log("DataManager: No PlayerPrefs save found");
            return false;
        }
        
        try
        {
            string jsonData = PlayerPrefs.GetString(PLAYER_PREFS_SAVE_KEY);
            currentSaveData = JsonUtility.FromJson<SaveGameData>(jsonData);
            
            hasUnsavedChanges = false;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: PlayerPrefs load failed - {e.Message}");
            return false;
        }
    }
    
    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }
    
    #endregion
    
    #region User Preferences API
    
    public void UpdateMusicVolume(float volume)
    {
        currentSaveData.userPreferences.musicVolume = Mathf.Clamp01(volume);
        hasUnsavedChanges = true;
        OnUserPreferencesChanged?.Invoke(currentSaveData.userPreferences);
    }
    
    public void UpdateSfxVolume(float volume)
    {
        currentSaveData.userPreferences.sfxVolume = Mathf.Clamp01(volume);
        hasUnsavedChanges = true;
        OnUserPreferencesChanged?.Invoke(currentSaveData.userPreferences);
    }
    
    public void UpdateInputBindings(string bindingsJson)
    {
        currentSaveData.userPreferences.inputBindingsJson = bindingsJson;
        hasUnsavedChanges = true;
        OnUserPreferencesChanged?.Invoke(currentSaveData.userPreferences);
    }
    
    public void SetShowTutorial(bool show)
    {
        currentSaveData.userPreferences.showTutorial = show;
        hasUnsavedChanges = true;
        OnUserPreferencesChanged?.Invoke(currentSaveData.userPreferences);
    }
    
    #endregion
    
    #region Game Progress API
    
    public void CompleteLevel(Scene levelId, int score, float timeInSeconds, int pageIndex = -1, int levelIndex = -1, bool isBossLevel = false, int diamondsCollected = 0)
    {
        var levelData = currentSaveData.gameProgress.GetLevelData(levelId);
        if (levelData != null)
        {
            currentSaveData.gameProgress.CompleteLevel(levelId, score, timeInSeconds, diamondsCollected);

            if (levelId != Scene.TutorialScene)
            {
                Debug.Log("Not tutorial scene, unlock next level");
                UnlockNextLevel(levelId, pageIndex, levelIndex, isBossLevel);
            }
            else
            {
                Debug.Log("Tutorial scene completed, no next level to unlock");
            }

            hasUnsavedChanges = true;
            OnLevelCompleted?.Invoke(levelData);
            OnGameProgressChanged?.Invoke(currentSaveData.gameProgress);
            
            Debug.Log($"DataManager: Level {levelId} completed with score {score} and time {timeInSeconds:F2}s");
        }
    }
    
    private void RecordLevelStart(Scene levelId, int pageIndex = -1, int levelIndex = -1, bool isBossLevel = false)
    {
        var levelData = currentSaveData.gameProgress.GetLevelData(levelId);
        if (levelData != null)
        {
            levelData.PlayLevel();
            currentSaveData.gameProgress.currentLevelId = levelId;
            hasUnsavedChanges = true;
            
            Debug.Log($"DataManager: Level {levelId} started at Page {pageIndex}, Index {levelIndex}, Boss: {isBossLevel}");
        }
    }
    
    public void RecordPlayerDeath()
    {
        currentSaveData.gameProgress.RecordDeath();
        hasUnsavedChanges = true;
        OnGameProgressChanged?.Invoke(currentSaveData.gameProgress);
    }
    
    public void UpdatePlayTime(float timeInSeconds)
    {
        currentSaveData.gameProgress.AddPlayTime(timeInSeconds);
        hasUnsavedChanges = true;
    }
    
    public void UpdateLevelCollectibles(Scene levelId, int collected)
    {
        currentSaveData.gameProgress.UpdateCollectibles(levelId, collected);
        hasUnsavedChanges = true;
        OnGameProgressChanged?.Invoke(currentSaveData.gameProgress);
    }
    
    public void StartLevel(Scene levelId)
    {
        var (pageIndex, levelIndex, isBossLevel) = GetLevelPosition(levelId);
        
        // Initialize current inventory as a fresh copy from persistent inventory
        var persistentInventoryItems = currentSaveData.gameProgress.GetInventoryItems();
        if (persistentInventoryItems == null || currentSaveData.gameProgress.isFirstTimePlaying)
        {
            Debug.Log("DataManager: first time playing, loading default items");
            persistentInventoryItems = new List<Item>();
            foreach (var itemSO in defaultListSO.items)
            {
                persistentInventoryItems.Add(new Item(itemSO, 1));
            }
            currentSaveData.gameProgress.SetInventoryItems(persistentInventoryItems);
            hasUnsavedChanges = true;
            currentSaveData.gameProgress.isFirstTimePlaying = false;
        }


        RecordLevelStart(levelId, pageIndex, levelIndex, isBossLevel);
        
        Debug.Log($"DataManager: Started level {levelId}");
    }
    
    public void CompleteCurrentLevel(int score, float timeInSeconds, int diamondsCollected)
    {
        if (currentSaveData.gameProgress.currentLevelId == Scene.None)
        {
            Debug.Log("DataManager: No current level to complete");
            return;
        }
        
        var levelId = currentSaveData.gameProgress.currentLevelId;
        
        // Determine level position efficiently  
        var (pageIndex, levelIndex, isBossLevel) = GetLevelPosition(levelId);
        
        // Complete the level
        CompleteLevel(levelId, score, timeInSeconds, pageIndex, levelIndex, isBossLevel, diamondsCollected);
        
        Debug.Log($"DataManager: Completed current level {levelId} with score {score} and time {timeInSeconds:F2}s");
    }

    public Scene GetNextLevelID()
    {
        if (currentSaveData.gameProgress.currentLevelId == Scene.TutorialScene)
        {
            return levelPageConfig.levelPages[0].NormalLevels[0].levelId;
        }

        var (pageIndex, levelIndex, isBossLevel) = GetLevelPosition(currentSaveData.gameProgress.currentLevelId);
        return GetNextLevelByPosition(pageIndex, levelIndex, isBossLevel);
    }
    
    private (int pageIndex, int levelIndex, bool isBossLevel) GetLevelPosition(Scene levelId)
    {
        if (levelPageConfig == null)
            return (-1, -1, false);
        
        // Search through pages to find level position
        for (int pageIndex = 0; pageIndex < levelPageConfig.levelPages.Count; pageIndex++)
        {
            var page = levelPageConfig.levelPages[pageIndex];
            
            // Check normal levels
            for (int levelIndex = 0; levelIndex < page.NormalLevels.Count; levelIndex++)
            {
                if (page.NormalLevels[levelIndex].levelId == levelId)
                {
                    return (pageIndex, levelIndex, false);
                }
            }
            
            // Check boss level
            if (page.BossLevel != null && page.BossLevel.levelId == levelId)
            {
                return (pageIndex, -1, true);
            }
        }
        
        return (-1, -1, false);
    }
    
    private void UnlockNextLevel(Scene completedLevelId, int pageIndex, int levelIndex, bool isBossLevel)
    {
        Scene nextLevelId = Scene.None;
        
        // Use efficient position-based approach if position data is available
        if (levelPageConfig != null && levelPageConfig.linearProgression && pageIndex >= 0)
        {
            nextLevelId = GetNextLevelByPosition(pageIndex, levelIndex, isBossLevel);
        }
        
        if (nextLevelId != Scene.None)
        {
            currentSaveData.gameProgress.UnlockLevel(nextLevelId);
            Debug.Log($"DataManager: Unlocked next level: {nextLevelId}");
        }
    }
    
    private Scene GetNextLevelByPosition(int pageIndex, int levelIndex, bool isBossLevel)
    {
        if (levelPageConfig == null || pageIndex >= levelPageConfig.levelPages.Count)
            return Scene.None;
        
        var currentPage = levelPageConfig.levelPages[pageIndex];
        
        if (!isBossLevel)
        {
            // Current level is a normal level
            if (levelIndex + 1 < currentPage.NormalLevels.Count)
            {
                // Next normal level in same page
                return currentPage.NormalLevels[levelIndex + 1].levelId;
            }
            else if (currentPage.BossLevel != null)
            {
                // Boss level in same page
                return currentPage.BossLevel.levelId;
            }
            else if (pageIndex + 1 < levelPageConfig.levelPages.Count)
            {
                // First level of next page
                var nextPage = levelPageConfig.levelPages[pageIndex + 1];
                if (nextPage.NormalLevels.Count > 0)
                {
                    return nextPage.NormalLevels[0].levelId;
                }
            }
        }
        else
        {
            // Current level is a boss level
            if (pageIndex + 1 < levelPageConfig.levelPages.Count)
            {
                // First level of next page
                var nextPage = levelPageConfig.levelPages[pageIndex + 1];
                if (nextPage.NormalLevels.Count > 0)
                {
                    return nextPage.NormalLevels[0].levelId;
                }
            }
        }
        
        return Scene.None; // No next level
    }

    public void UpdatePersistentInventory()
    {
        currentSaveData.gameProgress.SetInventoryItems(CurrentPlayerInventoryItems);
        hasUnsavedChanges = true;
        Debug.Log($"Won: update persistent inventory {CurrentPlayerInventoryItems.Count}");
    }
    public List<Item> GetPersistentInventoryCopy()
    {
        List<Item> items = new();
        var persistentInventory = currentSaveData.gameProgress.GetInventoryItems();
        foreach (var item in persistentInventory)
        {
            items.Add(new Item(item.itemSO, item.quantity));
        }
        return items;
    }

    #endregion

    #region Page-based Level Management

    public bool IsPageUnlocked(int pageIndex)
    {
        if (levelPageConfig == null || pageIndex >= levelPageConfig.levelPages.Count)
            return false;
        
        // First page is always unlocked
        if (pageIndex == 0)
            return true;
        
        // Check if previous page is completed
        return IsPageCompleted(pageIndex - 1);
    }
    
    public bool IsPageCompleted(int pageIndex)
    {
        if (levelPageConfig == null || pageIndex >= levelPageConfig.levelPages.Count)
            return false;
        
        var page = levelPageConfig.levelPages[pageIndex];
        
        // Check if all normal levels are completed
        foreach (var level in page.NormalLevels)
        {
            var levelData = currentSaveData?.gameProgress?.GetLevelData(level.levelId);
            if (levelData == null || !levelData.isCompleted)
                return false;
        }
        
        // Check if boss level is completed (if exists)
        if (page.BossLevel != null)
        {
            var bossData = currentSaveData?.gameProgress?.GetLevelData(page.BossLevel.levelId);
            if (bossData == null || !bossData.isCompleted)
                return false;
        }
        
        return true;
    }
    
    public int GetNumberOfPages()
    {
        return levelPageConfig.levelPages.Count;
    }

    public LevelPageConfigSO.LevelPage GetCurrentPage(int pageIndex)
    {
        return levelPageConfig.GetPage(pageIndex);
    }

    public float GetPageCompletionPercentage(int pageIndex)
    {
        if (levelPageConfig == null || pageIndex >= levelPageConfig.levelPages.Count)
            return 0f;
        
        var page = levelPageConfig.levelPages[pageIndex];
        int totalLevels = page.NormalLevels.Count + (page.BossLevel != null ? 1 : 0);
        
        if (totalLevels == 0)
            return 100f;
        
        int completedLevels = 0;
        
        // Count completed normal levels
        foreach (var level in page.NormalLevels)
        {
            var levelData = currentSaveData?.gameProgress?.GetLevelData(level.levelId);
            if (levelData != null && levelData.isCompleted)
                completedLevels++;
        }
        
        // Count boss level if completed
        if (page.BossLevel != null)
        {
            var bossData = currentSaveData?.gameProgress?.GetLevelData(page.BossLevel.levelId);
            if (bossData != null && bossData.isCompleted)
                completedLevels++;
        }
        
        return (float)completedLevels / totalLevels * 100f;
    }
    
    #endregion
    
    #region Utility Methods
    
    public bool HasSaveFile()
    {
        if (useJsonSaveFormat)
        {
            return File.Exists(GetSaveFilePath());
        }
        else
        {
            return PlayerPrefs.HasKey(PLAYER_PREFS_SAVE_KEY);
        }
    }
    
    public void DeleteSaveData()
    {
        try
        {
            if (useJsonSaveFormat)
            {
                string savePath = GetSaveFilePath();
                if (File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
                
                string backupPath = GetSaveFilePath() + BACKUP_SAVE_SUFFIX;
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }
            else
            {
                PlayerPrefs.DeleteKey(PLAYER_PREFS_SAVE_KEY);
                PlayerPrefs.Save();
            }
            
            // Reset current data
            currentSaveData = new SaveGameData();
            InitializeLevelData();
            
            Debug.Log("DataManager: Save data deleted and reset");
        }
        catch (Exception e)
        {
            Debug.LogError($"DataManager: Failed to delete save data - {e.Message}");
        }
    }

    public void ResetUserPreferences()
    {
        currentSaveData.userPreferences = new();
        
        hasUnsavedChanges = true;
        OnUserPreferencesChanged?.Invoke(currentSaveData.userPreferences);
        
        Debug.Log("DataManager: User preferences reset to defaults");
    }
    
    public void ForceImmediateSave()
    {
        if (hasUnsavedChanges)
        {
            SaveGameData();
        }
    }
    
    // Get statistics for UI display
    public Dictionary<string, object> GetGameStatistics()
    {
        var stats = new Dictionary<string, object>();
        
        if (currentSaveData?.gameProgress != null)
        {
            var progress = currentSaveData.gameProgress;
            
            stats["TotalScore"] = progress.totalScore;
            stats["TotalPlayTime"] = progress.GetFormattedTotalPlayTime();
            stats["LevelsCompleted"] = progress.levelsCompleted;
            stats["TotalLevels"] = progress.totalLevels;
            stats["CompletionPercentage"] = progress.OverallCompletionPercentage;
            stats["CollectiblesFound"] = progress.totalCollectiblesFound;
            stats["TotalCollectibles"] = progress.totalCollectibles;
            stats["CollectiblesPercentage"] = progress.CollectiblesCompletionPercentage;
            stats["TotalDeaths"] = progress.totalDeaths;
            stats["TotalWins"] = progress.totalWins;
            
            if (progress.lastPlayedDate != DateTime.MinValue)
            {
                stats["LastPlayed"] = progress.lastPlayedDate.ToString("yyyy-MM-dd HH:mm");
            }
        }
        
        return stats;
    }

    #endregion
}
