using System;

[System.Serializable]
public class LevelData
{
    public Scene levelId; // Scene name or unique identifier
    public string levelName;
    public bool isCompleted;
    public bool isUnlocked;
    public int bestScore;
    public int bestNumberOfDiamondsCollected;
    public float bestTime; // in seconds
    public DateTime firstCompletedDate;
    public DateTime lastPlayedDate;
    public int timesPlayed;
    public bool isBossLevel;
    
    // Level-specific achievements/collectibles
    public int totalCollectibles;
    public int collectedItems;
    public bool AllCollectiblesFound => collectedItems >= totalCollectibles;
    
    // Completion stats
    public float CompletionPercentage => totalCollectibles > 0 ? (float)collectedItems / totalCollectibles * 100f : 0f;
    
    public LevelData()
    {
        levelId = Scene.None;
        levelName = "";
        isCompleted = false;
        isUnlocked = false;
        bestScore = 0;
        bestNumberOfDiamondsCollected = 0;
        bestTime = float.MaxValue;
        firstCompletedDate = DateTime.MinValue;
        lastPlayedDate = DateTime.MinValue;
        timesPlayed = 0;
        isBossLevel = false;
        totalCollectibles = 0;
        collectedItems = 0;
    }
    
    public LevelData(Scene id, string name, bool bossLevel = false)
    {
        levelId = id;
        levelName = name;
        isCompleted = false;
        isUnlocked = false;
        bestScore = 0;
        bestNumberOfDiamondsCollected = 0;
        bestTime = 0;
        firstCompletedDate = DateTime.MinValue;
        lastPlayedDate = DateTime.MinValue;
        timesPlayed = 0;
        isBossLevel = bossLevel;
        totalCollectibles = 0;
        collectedItems = 0;
    }
    
    public void UpdateScore(int newScore)
    {
        if (newScore > bestScore)
        {
            bestScore = newScore;
        }
    }
    
    public void UpdateTime(float newTime)
    {
        if (newTime < bestTime)
        {
            bestTime = newTime;
        }
    }

    public void UpdateDiamondsCollected(int diamonds)
    {
        if (diamonds > bestNumberOfDiamondsCollected)
        {
            bestNumberOfDiamondsCollected = diamonds;
        }
    }

    public void CompleteLevel(int score, float time, int diamondsCollected)
    {
        if (!isCompleted)
        {
            isCompleted = true;
            firstCompletedDate = DateTime.Now;
        }
        
        UpdateScore(score);
        UpdateTime(time);
        UpdateDiamondsCollected(diamondsCollected);
        lastPlayedDate = DateTime.Now;
        timesPlayed++;
    }
    
    public void PlayLevel()
    {
        lastPlayedDate = DateTime.Now;
        timesPlayed++;
    }
    
    public string GetFormattedBestTime()
    {
        if (bestTime >= float.MaxValue)
            return "--:--";
            
        TimeSpan timeSpan = TimeSpan.FromSeconds(bestTime);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}
