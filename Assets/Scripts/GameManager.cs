using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Existing fields...

    private BoxPoint[] boxPoints;
    private PlayerPoint[] playerPoints;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        boxPoints = FindObjectsByType<BoxPoint>(FindObjectsSortMode.None);
        playerPoints = FindObjectsByType<PlayerPoint>(FindObjectsSortMode.None);
    }

    private void Start()
    {
        if (boxPoints.Length == 0 || playerPoints.Length == 0)
        {
            Debug.LogWarning("No BoxPoint or PlayerPoint found. Check that they are in the scene and set as triggers.");
        }
        InvokeRepeating(nameof(CheckLevelCompletion), 0.1f, 0.5f);
    }

    private void CheckLevelCompletion()
    {
        if (AreAllPointsOccupied())
        {
            Debug.Log("Level complete!");
            LevelFinished();
        }
    }

    private bool AreAllPointsOccupied()
    {
        foreach (BoxPoint bp in boxPoints)
        {
            if (!bp.isOccupied) return false;
        }
        foreach (PlayerPoint pp in playerPoints)
        {
            if (!pp.isOccupied) return false;
        }
        return true;
    }

    private void LoadLevelEnd() => SceneManager.LoadScene("TheEnd");

    public void LevelFinished()
    {
        UI_InGame.instance.fadeEffect.ScreenFadeEffect(1, 1.5f, LoadLevelEnd);
        CancelInvoke(nameof(CheckLevelCompletion));
    }

    // Add this method if you wish to trigger completion check manually.
    public void CheckPuzzleCompletion(Transform playerTransform, Transform boxTransform)
    {
        // You can implement additional checks here if needed,
        // or simply call LevelFinished if the scene is already correctly set.
        if (AreAllPointsOccupied())
        {
            Debug.Log("Puzzle complete via manual check.");
            LevelFinished();
        }
        else
        {
            Debug.Log("Puzzle not complete.");
        }
    }
}