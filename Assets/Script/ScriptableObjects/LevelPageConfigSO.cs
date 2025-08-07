using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelPageConfig", menuName = "Data Manager/Level Page Config SO")]
public class LevelPageConfigSO : ScriptableObject
{
    [System.Serializable]
    public class LevelInfo
    {
        [Header("Level Identity")]
        public Scene levelId; // TODO: reconsider later
        public string displayName;
        public int levelNumber;
        
        [Header("Level Properties")]
        public int totalCollectibles = 10;
        public bool unlockedByDefault = false;
        public List<string> prerequisiteLevels = new();
    }
    
    [System.Serializable]
    public class LevelPage
    {
        [Header("Page Info")]
        public string pageName;
        public int pageNumber;
        
        [Header("Levels")]
        [SerializeField] private List<LevelInfo> normalLevels = new();
        [SerializeField] private LevelInfo bossLevel;
        
        public List<LevelInfo> NormalLevels => normalLevels;
        public LevelInfo BossLevel => bossLevel;
    }
    
    [Header("Level Pages Configuration")]
    public List<LevelPage> levelPages = new();
    
    [Header("Default Settings")]
    public bool autoUnlockFirstLevel = true;
    public bool linearProgression = true; // Unlock next level after completing current
    public bool unlockAllLevel = true;

    #region Helper methods

    public LevelPage GetPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < levelPages.Count)
            return levelPages[pageIndex];
        return null;
    }
    
    
    [ContextMenu("Validate Configuration")]
    public void ValidateConfiguration()
    {
        Debug.Log("=== Level Page Configuration Validation ===");
        
        var allIds = new HashSet<Scene>();
        int totalLevels = 0;
        
        for (int p = 0; p < levelPages.Count; p++)
        {
            var page = levelPages[p];
            Debug.Log($"Page {p + 1}: {page.pageName} - {page.NormalLevels.Count} normal levels + 1 boss");
            
            // Check normal levels
            foreach (var level in page.NormalLevels)
            {
                if (allIds.Contains(level.levelId))
                    Debug.LogError($"Duplicate level ID: {level.levelId}");
                else
                    allIds.Add(level.levelId);

                totalLevels++;
            }
            
            // Check boss level
            if (page.BossLevel != null)
            {
                if (allIds.Contains(page.BossLevel.levelId))
                    Debug.LogError($"Duplicate boss ID: {page.BossLevel.levelId}");
                else
                    allIds.Add(page.BossLevel.levelId);
                    
                totalLevels++;
            }
        }
        
        Debug.Log($"Validation complete: {levelPages.Count} pages, {totalLevels} total levels");
    }
    
    #endregion
}
