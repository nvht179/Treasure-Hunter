using System;
using UnityEngine;

[System.Serializable]
public class UserPreferencesData
{
    [Header("Audio Settings")]
    public float musicVolume = 0.1f;
    public float sfxVolume = 1.0f;
    
    [Header("Input Bindings")]
    public string inputBindingsJson = "";
    
    [Header("General Settings")]
    public bool showTutorial = true;
    public int selectedLanguage = 0; // for future localization
    
    // Constructor with default values
    public UserPreferencesData()
    {
        musicVolume = 0.1f;
        sfxVolume = 1.0f;
        inputBindingsJson = "";
        showTutorial = true;
        selectedLanguage = 0;
    }
    
    // Copy constructor
    public UserPreferencesData(UserPreferencesData other)
    {
        musicVolume = other.musicVolume;
        sfxVolume = other.sfxVolume;
        inputBindingsJson = other.inputBindingsJson;
        showTutorial = other.showTutorial;
        selectedLanguage = other.selectedLanguage;
    }
}
