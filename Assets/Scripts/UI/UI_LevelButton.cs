using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumberText;

    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private TextMeshProUGUI totalTimeText;
    
    private int levelIndex;
    private string sceneName;

    public void SetupButton(int newLevelIndex)
    {
        levelIndex = newLevelIndex;
        levelNumberText.text = "Level " + levelIndex;
        sceneName = "Level_" + levelIndex;

        bestTimeText.text = TimerInfoText();
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }

    private string TimerInfoText()
    {
        float timerValue = PlayerPrefs.GetFloat("Level" + levelIndex + "BestTime", 999);
        if (Mathf.Approximately(timerValue, 999))
            return "Best Time: --:--";
        
        int minutes = Mathf.FloorToInt(timerValue / 60f);
        int seconds = Mathf.FloorToInt(timerValue % 60f);
        return $"Best Time: {minutes:00}:{seconds:00}";
    }
}
