using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI boxText;
    [SerializeField] private TextMeshProUGUI playerText;

    [SerializeField] private GameObject pauseUI;

    private bool isPaused;

    private void Awake()
    {
        instance = this;
        
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }
    }

    public void PauseButton()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1;
            pauseUI.SetActive(false);
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0;
            pauseUI.SetActive(true);
        }
    }

    public void GoToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void UpdateBoxUI(int occupiedBoxes, int totalBoxes)
    {
        boxText.text = occupiedBoxes + "/" + totalBoxes;
    }
    
    public void UpdatePlayerUI(int occupiedPoints, int totalPoints)
    {
        playerText.text = occupiedPoints + "/" + totalPoints;
    }

    public void UpdateTimerUI(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = $"Remaining time: {minutes:00}:{seconds:00}";
    }
}
