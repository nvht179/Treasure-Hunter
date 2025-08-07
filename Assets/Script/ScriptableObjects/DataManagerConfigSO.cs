using UnityEngine;

[CreateAssetMenu(fileName = "DataManagerConfig", menuName = "Data Manager/Data Manager Config SO")]
public class DataManagerConfigSO : ScriptableObject
{
    [Header("Save System Settings")]
    [Tooltip("Use JSON files instead of PlayerPrefs for saving")]
    public bool useJsonSaveFormat = true;
    
    [Tooltip("Automatically save data at regular intervals")]
    public bool enableAutoSave = true;
    
    [Tooltip("Time between auto-saves in seconds")]
    [Range(10f, 300f)]
    public float autoSaveInterval = 30f;
    
    [Header("Default User Preferences")]
    [Range(0f, 1f)]
    public float defaultMusicVolume = 0.3f;
    
    [Range(0f, 1f)]
    public float defaultSfxVolume = 1.0f;
    
    public bool defaultShowTutorial = true;
    
    [Header("Debug Settings")]
    [Tooltip("Enable debug logging for save/load operations")]
    public bool enableDebugLogging = false;
    
    [Tooltip("Create backup files when saving")]
    public bool createBackupFiles = true;
    
    [ContextMenu("Reset to Defaults")]
    public void ResetToDefaults()
    {
        useJsonSaveFormat = true;
        enableAutoSave = true;
        autoSaveInterval = 30f;
        defaultMusicVolume = 0.3f;
        defaultSfxVolume = 1.0f;
        defaultShowTutorial = true;
        enableDebugLogging = false;
        createBackupFiles = true;
        
        Debug.Log("DataManagerConfigSO reset to default values");
    }
}
