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
    
    private Dictionary<int, float> levelMaxTimes = new()
    {
        {1, 120f},
        {2, 150f},
        {3, 300f},
    };
    
    [Header("Rewards")]
    private Dictionary<int, int> levelRewards = new()
    {
        {1, 10},
        {2, 15},
        {3, 20},
    };

    private BoxPoint[] boxPoints;
    private PlayerPoint[] playerPoints;
    private bool levelFinishProcessed = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset everything since there's a bug and this is the only way i can deal with it
        Time.timeScale = 1;
        Input.ResetInputAxes();
        
        ResetPlayerStates();
    }

    private void ResetPlayerStates()
    {
        // Reset Player and also if you were wondering Player[] for future local co op function so yeah i'm not dumb
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            Debug.Log("Resetting player: " + player.gameObject.name);
            player.ResetState();
        }

        // Reset PlayerBoxInteraction
        PlayerBoxInteraction[] interactions = FindObjectsByType<PlayerBoxInteraction>(FindObjectsSortMode.None);
        foreach (PlayerBoxInteraction interaction in interactions)
        {
            Debug.Log("Resetting PlayerBoxInteraction: " + interaction.gameObject.name);
            interaction.ResetState();
        }
        
        StopAllCoroutines();
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
        inGameUI.fadeEffect.ScreenFade(1, 1.5f, ReturnToMainMenu);
    }

    public void CheckLevelCompletion()
    {
        if (!levelFinishProcessed && AreAllPointsOccupied())
        {
            levelFinishProcessed = true;
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
        AddLevelReward();
        SaveLevelProgression();
        SaveBestTime();
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
        int reward = 5;
        
        float bestTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 999);

        if (Mathf.Approximately(bestTime, 999))
        {
            if (levelRewards.ContainsKey(currentLevelIndex))
                reward = levelRewards[currentLevelIndex];
            else
                reward = 5;
        }
        else
            reward = 5;
        
        PlayerPrefs.SetInt("MoneyInBank", currentMoney + reward);
        PlayerPrefs.Save();
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