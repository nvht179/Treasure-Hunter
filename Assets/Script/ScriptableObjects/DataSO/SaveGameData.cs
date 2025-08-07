using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGameData
{
    [Header("Save Info")]
    public string saveVersion = "1.0";
    public DateTime saveDate;
    public string saveName;
    
    [Header("Game Data")]
    public UserPreferencesData userPreferences;
    public GameProgressData gameProgress;
    
    public SaveGameData()
    {
        saveVersion = "1.0";
        saveDate = DateTime.Now;
        saveName = "TreasureHunter_Save";
        userPreferences = new UserPreferencesData();
        gameProgress = new GameProgressData();
    }
    
    public SaveGameData(UserPreferencesData preferences, GameProgressData progress)
    {
        saveVersion = "1.0";
        saveDate = DateTime.Now;
        saveName = "TreasureHunter_Save";
        userPreferences = new UserPreferencesData(preferences);
        gameProgress = progress;
    }
    
    public void UpdateSaveDate()
    {
        saveDate = DateTime.Now;
    }
}
