using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Level Management")]
    [SerializeField] private int currentLevelIndex;

    private BoxPoint[] boxPoints;
    private PlayerPoint[] playerPoints;
    private bool levelFinishProcessed = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset for the new level
        levelFinishProcessed = false;
        
        // Update level data
        currentLevelIndex = scene.buildIndex;
        
        // Find points in the new scene
        RefreshPointsReferences();
        
        // Only cancel and restart if we're in a level scene (not menu or end)
        if (scene.name.StartsWith("Level_"))
        {
            CancelInvoke(nameof(CheckLevelCompletion));
            InvokeRepeating(nameof(CheckLevelCompletion), 0.1f, 0.5f);
        }
    }
    
    private void RefreshPointsReferences()
    {
        boxPoints = FindObjectsByType<BoxPoint>(FindObjectsSortMode.None);
        playerPoints = FindObjectsByType<PlayerPoint>(FindObjectsSortMode.None);
        Debug.Log($"Found {boxPoints.Length} box points and {playerPoints.Length} player points");
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void CheckLevelCompletion()
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

    private void LoadLevelEnd() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel()
    {
        int nextLevelIndex = currentLevelIndex + 1;
        SceneManager.LoadScene("Level_" + nextLevelIndex);
    }

    private void LevelFinished()
    {
        CancelInvoke(nameof(CheckLevelCompletion));
        var fadeEffect = UI_InGame.instance.fadeEffect;

        // Menu and End screen take 2 that's why total scene = all level scene + 2
        var noMoreLevels = currentLevelIndex + 2 >= SceneManager.sceneCountInBuildSettings;

        if (noMoreLevels)
            fadeEffect.ScreenFade(1, 1.5f, LoadLevelEnd);
        else
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);
    }

    // This can be called by PlayerPoint when player enters
    public void TriggerLevelCompletionCheck()
    {
        CheckLevelCompletion();
    }
}