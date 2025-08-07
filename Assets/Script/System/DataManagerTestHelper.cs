using UnityEngine;
using System.Collections.Generic;

public class DataManagerTestHelper : MonoBehaviour
{
    [Header("Testing Tools")]
    [SerializeField] private bool enableDebugKeys = true;
    
    [Header("Test Data")]
    [SerializeField] private Scene testLevelId = Scene.PalmTreeIslandScene;
    [SerializeField] private int testScore = 1000;
    [SerializeField] private float testTime = 120f;
    
    private void Update()
    {
        if (!enableDebugKeys || DataManager.Instance == null)
            return;
            
        // Debug keys for testing (only in development)
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        // F1 - Complete test level
        if (Input.GetKeyDown(KeyCode.F1))
        {
            CompleteTestLevel();
        }
        
        // F2 - Add random score
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(Random.Range(100, 500));
            }
        }
        
        // F3 - Print statistics
        if (Input.GetKeyDown(KeyCode.F3))
        {
            PrintGameStatistics();
        }
        
        // F4 - Force save
        if (Input.GetKeyDown(KeyCode.F4))
        {
            DataManager.Instance.ForceImmediateSave();
            Debug.Log("DataManager: Force save completed");
        }
        
        // F5 - Reset save data (DANGER!)
        if (Input.GetKeyDown(KeyCode.F5) && Input.GetKey(KeyCode.LeftShift))
        {
            DataManager.Instance.DeleteSaveData();
            Debug.Log("DataManager: Save data reset!");
        }
        
        // F6 - Unlock all levels
        if (Input.GetKeyDown(KeyCode.F6))
        {
            UnlockAllLevels();
        }
        
        // F7 - Validate Level Page Configuration
        if (Input.GetKeyDown(KeyCode.F7))
        {
            ValidateLevelPageConfiguration();
        }
        
        // F8 - Test Scene Loading
        if (Input.GetKeyDown(KeyCode.F8))
        {
            TestLoadScene();
        }
        
        // F9 - Test Page-based Level Progression
        if (Input.GetKeyDown(KeyCode.F9))
        {
            TestPageBasedProgression();
        }
        
        #endif
    }
    
    [ContextMenu("Complete Test Level")]
    public void CompleteTestLevel()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.CompleteLevel(testLevelId, testScore, testTime);
            Debug.Log($"Test: Completed level {testLevelId} with score {testScore} and time {testTime}s");
        }
    }
    
    [ContextMenu("Print Game Statistics")]
    public void PrintGameStatistics()
    {
        if (DataManager.Instance == null)
        {
            Debug.Log("DataManager not available");
            return;
        }
        
        var stats = DataManager.Instance.GetGameStatistics();
        Debug.Log("=== GAME STATISTICS ===");
        
        foreach (var stat in stats)
        {
            Debug.Log($"{stat.Key}: {stat.Value}");
        }
        
        Debug.Log("=== LEVEL PROGRESS ===");
        if (DataManager.Instance.GameProgress?.levelDataList != null)
        {
            foreach (var level in DataManager.Instance.GameProgress.levelDataList)
            {
                string status = level.isCompleted ? "COMPLETED" : (level.isUnlocked ? "UNLOCKED" : "LOCKED");
                Debug.Log($"{level.levelName} ({level.levelId}): {status}");
                
                if (level.isCompleted)
                {
                    Debug.Log($"  - Best Score: {level.bestScore}");
                    Debug.Log($"  - Best Time: {level.GetFormattedBestTime()}");
                    Debug.Log($"  - Times Played: {level.timesPlayed}");
                    Debug.Log($"  - Collectibles: {level.collectedItems}/{level.totalCollectibles}");
                }
            }
        }
    }
    
    [ContextMenu("Unlock All Levels")]
    public void UnlockAllLevels()
    {
        if (DataManager.Instance?.GameProgress?.levelDataList == null)
        {
            Debug.LogWarning("No level data available");
            return;
        }
        
        foreach (var level in DataManager.Instance.GameProgress.levelDataList)
        {
            level.isUnlocked = true;
        }
        
        Debug.Log("All levels unlocked!");
    }
    
    [ContextMenu("Simulate Play Session")]
    public void SimulatePlaySession()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManager not available");
            return;
        }
        
        // Simulate some gameplay data
        var levels = new List<Scene> { Scene.TutorialScene, Scene.PalmTreeIslandScene, Scene.PirateShipScene };
        
        foreach (var levelId in levels)
        {
            // Random score and time
            int score = Random.Range(500, 2000);
            float time = Random.Range(60f, 300f);
            
            DataManager.Instance.CompleteLevel(levelId, score, time);
            
            // Random collectibles
            int collectibles = Random.Range(5, 10);
            DataManager.Instance.UpdateLevelCollectibles(levelId, collectibles);
        }
        
        // Add some deaths
        for (int i = 0; i < Random.Range(2, 8); i++)
        {
            DataManager.Instance.RecordPlayerDeath();
        }
        
        // Add play time
        DataManager.Instance.UpdatePlayTime(Random.Range(600f, 1800f));
        
        Debug.Log("Simulated play session completed!");
    }
    
    [ContextMenu("Test Page-based Progression")]
    public void TestPageBasedProgression()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("DataManager not available");
            return;
        }
        
        Debug.Log("=== TESTING PAGE-BASED PROGRESSION ===");
        
        // Find LevelPageConfigSO being used
        var chooseLevelUI = FindObjectOfType<ChooseLevelUI>();
        if (chooseLevelUI != null)
        {
            var configField = typeof(ChooseLevelUI).GetField("levelPageConfig", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (configField != null)
            {
                var config = configField.GetValue(chooseLevelUI) as LevelPageConfigSO;
                if (config != null)
                {
                    Debug.Log($"Found LevelPageConfigSO: {config.name}");
                    Debug.Log($"Total Pages: {config.levelPages.Count}");
                    
                    // Test progression logic
                    for (int pageIndex = 0; pageIndex < config.levelPages.Count; pageIndex++)
                    {
                        var page = config.levelPages[pageIndex];
                        bool pageUnlocked = DataManager.Instance.IsPageUnlocked(pageIndex);
                        bool pageCompleted = DataManager.Instance.IsPageCompleted(pageIndex);
                        
                        Debug.Log($"Page {pageIndex + 1} ({page.pageName}): " +
                                 $"Unlocked={pageUnlocked}, Completed={pageCompleted}");
                    }
                }
                else
                {
                    Debug.LogWarning("No LevelPageConfigSO assigned to ChooseLevelUI");
                }
            }
        }
        else
        {
            Debug.LogWarning("ChooseLevelUI not found in scene");
        }
    }
    
    [ContextMenu("Validate Level Page Configuration")]
    public void ValidateLevelPageConfiguration()
    {
        var chooseLevelUI = FindObjectOfType<ChooseLevelUI>();
        if (chooseLevelUI != null)
        {
            var configField = typeof(ChooseLevelUI).GetField("levelPageConfig", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (configField != null)
            {
                var config = configField.GetValue(chooseLevelUI) as LevelPageConfigSO;
                if (config != null)
                {
                    Debug.Log("=== VALIDATING LEVEL PAGE CONFIGURATION ===");
                    config.ValidateConfiguration();
                }
                else
                {
                    Debug.LogWarning("No LevelPageConfigSO assigned to ChooseLevelUI");
                }
            }
        }
        else
        {
            Debug.LogWarning("ChooseLevelUI not found in scene");
        }
    }
    
    [ContextMenu("Test Load Scene")]
    public void TestLoadScene()
    {
        // Test loading via SceneLoader
        Scene sceneToLoad = testLevelId;
        
        if (sceneToLoad != Scene.MainMenuScene) // MainMenuScene used as "not found" indicator
        {
            Debug.Log($"[TEST] Loading scene {sceneToLoad} for level ID {testLevelId}");
            SceneLoader.Load(sceneToLoad);
        }
        else
        {
            Debug.LogError($"[TEST] Cannot map level ID '{testLevelId}' to a SceneLoader scene");
        }
    }
    private void OnGUI()
    {
        if (!enableDebugKeys)
            return;
            
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        GUILayout.BeginArea(new Rect(10, 10, 350, 280));
        GUILayout.Label("Data Manager Debug Tools");
        GUILayout.Label("F1 - Complete Test Level");
        GUILayout.Label("F2 - Add Random Score");
        GUILayout.Label("F3 - Print Statistics");
        GUILayout.Label("F4 - Force Save");
        GUILayout.Label("F5+Shift - Reset Save (DANGER!)");
        GUILayout.Label("F6 - Unlock All Levels");
        GUILayout.Label("F7 - Validate Level Config");
        GUILayout.Label("F8 - Test Scene Load");
        GUILayout.Label("F9 - Test Page Progression");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Simulate Play Session"))
        {
            SimulatePlaySession();
        }
        
        // Show current page status if available
        if (DataManager.Instance != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("=== PAGE STATUS ===");
            
            var chooseLevelUI = FindObjectOfType<ChooseLevelUI>();
            if (chooseLevelUI != null)
            {
                var configField = typeof(ChooseLevelUI).GetField("levelPageConfig", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (configField != null)
                {
                    var config = configField.GetValue(chooseLevelUI) as LevelPageConfigSO;
                    if (config != null)
                    {
                        for (int i = 0; i < Mathf.Min(3, config.levelPages.Count); i++) // Show first 3 pages
                        {
                            bool unlocked = DataManager.Instance.IsPageUnlocked(i);
                            bool completed = DataManager.Instance.IsPageCompleted(i);
                            string status = completed ? "✓" : (unlocked ? "○" : "●");
                            GUILayout.Label($"Page {i + 1}: {status} {config.levelPages[i].pageName}");
                        }
                        
                        if (config.levelPages.Count > 3)
                        {
                            GUILayout.Label($"... and {config.levelPages.Count - 3} more pages");
                        }
                    }
                }
            }
        }
        
        GUILayout.EndArea();
        
        #endif
    }
}
