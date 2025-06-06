using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Boosters : MonoBehaviour
{
    [Header("Booster Buttons")]
    [SerializeField] private Button[] boosterButtons; // All 8 booster buttons
    [SerializeField] private Button letsGoButton;
    
    // References to the freeze time and speed up buttons in the game UI
    [SerializeField] private Button freezeTimeButton;
    [SerializeField] private Button speedUpButton;
    [SerializeField] private Button checkPointButton;
    [SerializeField] private Button lastResortButton;
    
    private int selectedBoosterIndex = -1; // -1 means no selection
    private Color normalColor = new(1f, 1f, 1f, 0f);
    private Color selectedColor = new(1f, 1f, 1f, 1f);
    
    private void Awake()
    {
        // Pause the game when this UI appears
        Time.timeScale = 0;
        
        // Setup the Let's Go button (initially disabled)
        letsGoButton.interactable = false;
        
        // Setup all booster buttons
        for (int i = 0; i < boosterButtons.Length; i++)
        {
            int index = i; // Capture the index for the lambda
            boosterButtons[i].GetComponent<Image>().color = normalColor;
            boosterButtons[i].onClick.AddListener(() => SelectBooster(index));
        }
        
        // Setup Let's Go button
        letsGoButton.onClick.AddListener(StartGameWithSelectedBooster);
    }
    
    private void SelectBooster(int index)
    {
        // If this booster is already selected, do nothing
        if (selectedBoosterIndex == index)
            return;

        // Deselect the previous booster if any
        if (selectedBoosterIndex >= 0 && selectedBoosterIndex < boosterButtons.Length)
        {
            boosterButtons[selectedBoosterIndex].GetComponent<Image>().color = normalColor;
        }

        // Select the new booster
        selectedBoosterIndex = index;
        boosterButtons[index].GetComponent<Image>().color = selectedColor;

        // Enable Let's Go button since we have a selection now
        letsGoButton.interactable = true;
    }

    private void StartGameWithSelectedBooster()
    {
        // Only proceed if we have a valid selection
        if (selectedBoosterIndex < 0 || selectedBoosterIndex >= boosterButtons.Length)
            return;

        // Resume the game
        Time.timeScale = 1;

        // Activate the selected booster
        ActivateBooster(selectedBoosterIndex);

        // Hide this UI
        gameObject.SetActive(false);
    }

    private void ActivateBooster(int index)
    {
        // Map the booster index to the actual booster effect
        switch (index)
        {
            case 0:
                // Activate Freeze Time booster
                if (freezeTimeButton != null)
                    freezeTimeButton.gameObject.SetActive(true);
                break;

            case 1:
                // Activate Speed Up booster
                if (speedUpButton != null)
                    speedUpButton.gameObject.SetActive(true);
                break;

            case 2:
                // Activate Checkpoint booster
                if (checkPointButton != null)
                    checkPointButton.gameObject.SetActive(true);
                break;

            case 3:
                // Activate Last Resort booster
                if (lastResortButton != null)
                    lastResortButton.gameObject.SetActive(true);
                break;
            
            default:
                Debug.LogWarning("Unknown booster index: " + index);
                break;
        }
    }
}
