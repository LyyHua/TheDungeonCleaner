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
    [SerializeField] private Button checkPointButton;
    [SerializeField] private Button lastResortButton;
    
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
        if (playerBoxInteraction == null)
        {
            Debug.LogWarning("PlayerBoxInteraction not found. Attempting fallback.");
            playerBoxInteraction = FindObjectOfType<PlayerBoxInteraction>();
        }

        // Ensure fadeEffect is assigned
        fadeEffect = GetComponentInChildren<UI_FadeEffect>(true);
        if (fadeEffect == null)
        {
            Debug.LogError("UI_FadeEffect is missing. Ensure it is added to the scene.");
        }

        // Ensure grabButtonImage is assigned
        if (grabButton != null)
        {
            grabButtonImage = grabButton.GetComponent<Image>();
        }
        else
        {
            Debug.LogError("GrabButton is missing. Ensure it is assigned in the inspector.");
        }

        // Ensure cooldown overlays are set up
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
        if (occupiedBoxes >= totalBoxes && totalBoxes > 0)
            boxText.color = Color.green;
        else
            boxText.color = Color.white;
    }
    
    public void UpdatePlayerUI(int occupiedPoints, int totalPoints)
    {
        playerText.text = occupiedPoints + "/" + totalPoints;
        if (occupiedPoints >= totalPoints && totalPoints > 0)
            playerText.color = Color.green;
        else
            playerText.color = Color.white;
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
        freezeTimeButton.gameObject.SetActive(false);
    }

    private void ResetTimerTextColor()
    {
        timerText.color = Color.white;
    }

    public void OnSpeedUpButtonPressed()
    {
        StartCoroutine(HandleSpeedUpBooster(60f));
        speedUpButton.gameObject.SetActive(false);
    }
    
    public void OnCheckPointButtonPressed()
    {
        checkPointButton.gameObject.SetActive(false);
    }

    public void OnLastResortButtonPressed()
    {
        lastResortButton.gameObject.SetActive(false);
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
