using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public string sceneName;
    
    [SerializeField] private GameObject[] uiElements;
    
    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }
    
    private void Start()
    {
        fadeEffect.ScreenFadeEffect(0, 1.5f);
    }

    public void SwitchUI(GameObject uiToEnable)
    {
        foreach (var uiElement in uiElements)
        {
            uiElement.SetActive(false);
        }
        
        uiToEnable.SetActive(true);
    }

    public void NewGame()
    {
        fadeEffect.ScreenFadeEffect(1, 1.5f, LoadLevelScene);
    }
    
    private void LoadLevelScene() => SceneManager.LoadScene(sceneName);
}
