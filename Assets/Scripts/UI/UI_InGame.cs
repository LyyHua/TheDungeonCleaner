using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI boxText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI grabText;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject grabButton;
    [SerializeField] private Button freezeTimeButton;
    [SerializeField] private Button speedUpButton;
    
    private bool freezeTimeOnCooldown = false;
    private bool speedUpOnCooldown = false;
    private float freezeTimeCooldownRemaining = 0f;
    private float speedUpCooldownRemaining = 0f;

    private Image grabButtonImage;
    private Image freezeTimeImage;
    private Image speedUpImage;

    private bool isPaused;
    private PlayerBoxInteraction playerBoxInteraction;

    private void Awake()
    {
        instance = this;
        playerBoxInteraction = FindFirstObjectByType<PlayerBoxInteraction>();
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        grabButtonImage = grabButton.GetComponent<Image>();
        freezeTimeImage = freezeTimeButton.GetComponent<Image>();
        speedUpImage = speedUpButton.GetComponent<Image>();
        
        SetupCooldownOverlay(freezeTimeImage);
        SetupCooldownOverlay(speedUpImage);
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
        UpdateGrabButtonState();
        UpdateCooldowns();
    }
    
    private void SetupCooldownOverlay(Image overlay)
    {
        if (overlay != null)
        {
            overlay.type = Image.Type.Filled;
            overlay.fillMethod = Image.FillMethod.Radial360;
            overlay.fillOrigin = 2;
            overlay.fillClockwise = false;
            overlay.fillAmount = 1f;
            overlay.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    
    private void UpdateCooldowns()
    {
        if (freezeTimeOnCooldown)
        {
            freezeTimeCooldownRemaining -= Time.deltaTime;
            freezeTimeImage.fillAmount = 1 - (freezeTimeCooldownRemaining / 60f);

            if (freezeTimeCooldownRemaining <= 0)
            {
                freezeTimeOnCooldown = false;
                freezeTimeImage.fillAmount = 1f;
                freezeTimeImage.color = new Color(1f, 1f, 1f, 1f);
                freezeTimeButton.interactable = true;
            }
        }
        
        if (speedUpOnCooldown)
        {
            speedUpCooldownRemaining -= Time.deltaTime;
            speedUpImage.fillAmount = 1 - (speedUpCooldownRemaining / 60f);

            if (speedUpCooldownRemaining <= 0)
            {
                speedUpOnCooldown = false;
                speedUpImage.fillAmount = 1f;
                speedUpImage.color = new Color(1f, 1f, 1f, 1f);
                speedUpButton.interactable = true;
            }
        }
    }
    
    private void UpdateGrabButtonState()
    {
        if (playerBoxInteraction.isDragging)
        {
            grabText.text = "Release";
            grabButtonImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else if (playerBoxInteraction.isBoxHighlighted)
        {
            grabText.text = "Grab";
            grabButtonImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            grabText.text = "Grab";
            grabButtonImage.color = new Color(1f, 1f, 1f, 0.6f);
        }
    }

    public void GrabReleaseButton()
    {
        if (playerBoxInteraction.isDragging)
        {
            if (!playerBoxInteraction.isMoving)
                playerBoxInteraction.ReleaseBox();
            else
            {
                playerBoxInteraction.BufferReleaseAction();
            }
        }
        else
        {
            if (!playerBoxInteraction.isMoving)
                playerBoxInteraction.TryGrabBox();
            else
            {
                playerBoxInteraction.BufferGrabAction();
            }
        }
    }
    
    public void UndoButton() => playerBoxInteraction.UndoMove();
    
    public void ResetLevelButton() => playerBoxInteraction.ResetLevel();

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
    
    public void OnFreezeTimeButtonPressed()
    {
        GameManager.instance.FreezeTime(60f);
        timerText.color = Color.cyan;
        Invoke(nameof(ResetTimerTextColor), 60f);
        
        freezeTimeOnCooldown = true;
        freezeTimeCooldownRemaining = 60f;
        freezeTimeButton.interactable = false;
        freezeTimeImage.color = new Color(1f, 1f, 1f, 1f);
        freezeTimeImage.fillAmount = 0f;
    }

    private void ResetTimerTextColor()
    {
        timerText.color = Color.white;
    }

    public void OnSpeedUpButtonPressed()
    {
        StartCoroutine(HandleSpeedUpBooster(60f));
        
        speedUpOnCooldown = true;
        speedUpCooldownRemaining = 60f;
        speedUpButton.interactable = false;
        speedUpImage.color = new Color(1f, 1f, 1f, 1f);
        speedUpImage.fillAmount = 0f;
    }

    private IEnumerator HandleSpeedUpBooster(float duration)
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            p.SetMovementSpeed(0.1f);
        }

        PlayerBoxInteraction[] interactions = FindObjectsByType<PlayerBoxInteraction>(FindObjectsSortMode.None);
        foreach (var interaction in interactions)
        {
            interaction.SetMoveDuration(0.1f);
        }

        yield return new WaitForSeconds(duration);
        
        foreach (var p in players)
        {
            p.SetMovementSpeed(0.135f);
        }

        foreach (var interaction in interactions)
        {
            interaction.SetMoveDuration(0.135f);
        }
    }
}
