using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameProgressData
{
    [Header("Player Stats")]
    public int totalScore;
    public float totalPlayTime; // in seconds
    public int levelsCompleted;
    public int totalLevels;
    
    [Header("Level Progress")]
    public List<LevelData> levelDataList;
    public Scene currentLevelId;
    public int currentPage; // for level selection UI
    
    [Header("Achievements & Stats")]
    [HideInInspector]
    public int totalCollectiblesFound;
    [HideInInspector]
    public int totalCollectibles;
    public int totalDeaths;
    public int totalWins;
    public DateTime lastPlayedDate;
    public DateTime firstPlayedDate;
    
    // Properties for easy access
    public float OverallCompletionPercentage => totalLevels > 0 ? (float)levelsCompleted / totalLevels * 100f : 0f;
    public float CollectiblesCompletionPercentage => totalCollectibles > 0 ? (float)totalCollectiblesFound / totalCollectibles * 100f : 0f;
    
    public GameProgressData()
    {
        totalScore = 0;
        totalPlayTime = 0f;
        levelsCompleted = 0;
        totalLevels = 0;
        levelDataList = new List<LevelData>();
        currentLevelId = Scene.None;
        currentPage = 0;
        totalCollectiblesFound = 0;
        totalCollectibles = 0;
        totalDeaths = 0;
        totalWins = 0;
        lastPlayedDate = DateTime.MinValue;
        firstPlayedDate = DateTime.MinValue;
    }
    
    // Level management methods
    public LevelData GetLevelData(Scene levelId)
    {
        return levelDataList.Find(level => level.levelId == levelId);
    }
    
    public bool IsLevelUnlocked(Scene levelId)
    {
        var levelData = GetLevelData(levelId);
        return levelData != null && levelData.isUnlocked;
    }
    
    public bool IsLevelCompleted(Scene levelId)
    {
        var levelData = GetLevelData(levelId);
        return levelData != null && levelData.isCompleted;
    }
    
    public void UnlockLevel(Scene levelId)
    {
        var levelData = GetLevelData(levelId);
        if (levelData != null)
        {
            levelData.isUnlocked = true;
        }
    }
    
    public void CompleteLevel(Scene levelId, int score, float time, int diamondsCollected)
    {
        var levelData = GetLevelData(levelId);
        if (levelData != null)
        {
            bool wasCompleted = levelData.isCompleted;
            levelData.CompleteLevel(score, time, diamondsCollected);
            
            // Update global stats
            if (!wasCompleted)
            {
                levelsCompleted++;
            }
            
            totalScore += score;
            totalWins++;
            lastPlayedDate = DateTime.Now;
            if (firstPlayedDate == DateTime.MinValue)
            {
                firstPlayedDate = DateTime.Now;
            }
        }
    }
    
    public void RecordDeath()
    {
        totalDeaths++;
        lastPlayedDate = DateTime.Now;
    }
    
    public void AddPlayTime(float timeInSeconds)
    {
        totalPlayTime += timeInSeconds;
    }
    
    public void UpdateCollectibles(Scene levelId, int collected)
    {
        var levelData = GetLevelData(levelId);
        if (levelData != null)
        {
            int difference = collected - levelData.collectedItems;
            levelData.collectedItems = collected;
            totalCollectiblesFound += difference;
        }
    }
    
    public string GetFormattedTotalPlayTime()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(totalPlayTime);
        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
    }
    
    // Get list of completed levels
    public List<LevelData> GetCompletedLevels()
    {
        return levelDataList.FindAll(level => level.isCompleted);
    }
    
    // Get list of unlocked but not completed levels
    public List<LevelData> GetUnlockedLevels()
    {
        return levelDataList.FindAll(level => level.isUnlocked && !level.isCompleted);
    }
}
