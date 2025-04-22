using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private UI_InGame inGameUI;

    [Header("Level Management")]
    [SerializeField] private float levelTimer;
    [SerializeField] private float maxLevelTime = 60f;
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;
    
    private Dictionary<int, float> levelMaxTimes = new Dictionary<int, float>()
    {
        {1, 120f},  // Level 1: 120 seconds (2 minute)
        {2, 150f},  // Level 2: 150 seconds (2.5 minutes)
        {3, 300f}, // Level 3: 300 seconds (5 minutes)
        // Add more levels as needed
    };
    
    [Header("Rewards")]
    private Dictionary<int, int> levelRewards = new Dictionary<int, int>()
    {
        {1, 10},  // Level 1 gives 10 coins
        {2, 15},  // Level 2 gives 15 coins
        {3, 20},  // Level 3 gives 20 coins
        // Add more levels as needed
    };

    private BoxPoint[] boxPoints;
    private PlayerPoint[] playerPoints;
    private bool levelFinishProcessed = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        currentLevelIndex = currentScene.buildIndex;
        nextLevelIndex = currentLevelIndex + 1;
        inGameUI = UI_InGame.instance;
        CollectGameInfo();
        
        InitializeLevelTimer();
    }

    private void CollectGameInfo()
    {
        boxPoints = FindObjectsByType<BoxPoint>(FindObjectsSortMode.None);
        playerPoints = FindObjectsByType<PlayerPoint>(FindObjectsSortMode.None);
        UpdatePlayerCount();
        UpdateBoxCount();
    }
    
    private void InitializeLevelTimer()
    {
        levelTimer = 0f;
        maxLevelTime = levelMaxTimes[currentLevelIndex];
    }

    private void Update()
    {
        if (levelTimer < maxLevelTime)
        {
            levelTimer += Time.deltaTime;
            float remainingTime = maxLevelTime - levelTimer;
            
            if (inGameUI != null)
                inGameUI.UpdateTimerUI(remainingTime);
                
            // Check for time out
            if (remainingTime <= 0f)
            {
                TimeOut();
            }
        }
    }
    
    public void UpdateBoxCount()
    {
        if (inGameUI == null || boxPoints == null) return;
    
        int occupiedBoxes = 0;
        foreach (BoxPoint bp in boxPoints)
        {
            if (bp != null && bp.isOccupied)
                occupiedBoxes++;
        }
    
        inGameUI.UpdateBoxUI(occupiedBoxes, boxPoints.Length);
    }

    public void UpdatePlayerCount()
    {
        
        if (inGameUI == null || playerPoints == null) return;
    
        int occupiedPlayers = 0;
        foreach (PlayerPoint pp in playerPoints)
        {
            if (pp != null && pp.isOccupied)
                occupiedPlayers++;
        }
    
        inGameUI.UpdatePlayerUI(occupiedPlayers, playerPoints.Length);
    }

    private void TimeOut()
    {
        CancelInvoke(nameof(CheckLevelCompletion));
        Debug.Log("Time's up! Returning to main menu");
        inGameUI.fadeEffect.ScreenFade(1, 1.5f, ReturnToMainMenu);
    }

    public void CheckLevelCompletion()
    {
        if (!levelFinishProcessed && AreAllPointsOccupied())
        {
            levelFinishProcessed = true;
            Debug.Log("Level complete!");
            LevelFinished();
        }
    }

    private bool AreAllPointsOccupied()
    {
        if (boxPoints == null || boxPoints.Length == 0 || 
            playerPoints == null || playerPoints.Length == 0)
        {
            return false;
        }

        foreach (BoxPoint bp in boxPoints)
        {
            if (bp == null || !bp.isOccupied) return false;
        }
        
        foreach (PlayerPoint pp in playerPoints)
        {
            if (pp == null || !pp.isOccupied) return false;
        }
        
        return true;
    }
    
    private void LevelFinished()
    {
        SaveLevelProgression();
        SaveBestTime();
        AddLevelReward();
        LoadNextScene();
    }

    private void SaveBestTime()
    {
        float lastTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 999);
        
        if (levelTimer < lastTime)
            PlayerPrefs.SetFloat("Level" + currentLevelIndex + "BestTime", levelTimer);
    }

    private void SaveLevelProgression()
    {
        PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1);

        if (!NoMoreLevels())
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex);
    }
    
    private void AddLevelReward()
    {
        int currentMoney = PlayerPrefs.GetInt("MoneyInBank", 0);
        int reward = 5; // Default reward
    
        if (levelRewards.ContainsKey(currentLevelIndex))
            reward = levelRewards[currentLevelIndex];
        
        // If player completes the level faster than previous best, add a time bonus
        float bestTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 999);
        if (levelTimer < bestTime)
        {
            reward += 5; // Bonus for beating your best time
        }
    
        PlayerPrefs.SetInt("MoneyInBank", currentMoney + reward);
        PlayerPrefs.Save();
    
        Debug.Log($"Added {reward} coins. Total: {currentMoney + reward}");
    }

    private void LoadLevelEnd() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel()
    {
        SceneManager.LoadScene("Level_" + nextLevelIndex);
    }
    
    private void LoadNextScene()
    {
        CancelInvoke(nameof(CheckLevelCompletion));
        var fadeEffect = inGameUI.fadeEffect;

        if (NoMoreLevels())
            fadeEffect.ScreenFade(1, 1.5f, LoadLevelEnd);
        else
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);        
    }
    
    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private bool NoMoreLevels()
    {
        // Menu and End screen take 2 that's why total scene = all level scene + 2
        return currentLevelIndex + 2 >= SceneManager.sceneCountInBuildSettings;
    }
}