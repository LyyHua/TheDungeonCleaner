using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    
    [SerializeField] private GameObject[] uiElements;
    [SerializeField] private GameObject continueButton;
    
    private static bool isContinueMode;
    
    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow,
            new RefreshRate() { numerator = 60, denominator = 1 });
        Time.timeScale = 1;
    }
    
    private void Start()
    {
        if (HasLevelProgression())
            continueButton.SetActive(true);
        
        fadeEffect.ScreenFade(0, 1.5f);
    }

    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (var uiElement in uiElements)
        {
            uiElement.SetActive(false);
        }
        
        uiToEnable.SetActive(true);
        
        AudioManager.instance.PlaySFX(6);
    }

    private bool HasLevelProgression()
    {
        bool hasLevelProgression = PlayerPrefs.GetInt("ContinueLevelNumber", 0) > 0;
        return hasLevelProgression;
    }

    public void ContinueGame()
    {
        int levelToLoad = PlayerPrefs.GetInt("ContinueLevelNumber", 0);
        SceneManager.LoadScene("Level_" + levelToLoad);
        AudioManager.instance.PlaySFX(6);
    }
}
