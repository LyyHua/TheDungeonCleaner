using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    [SerializeField] private bool[] levelsUnlocked;

    private void Start()
    {
        LoadLevelsInfo();
        CreateLevelButton();
    }

    private void CreateLevelButton()
    {
        var levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        // Start from 1 to skip the main menu scene
        for (int i = 1; i < levelsAmount; i++)
        {
            if (IsLevelUncloked(i) == false)
                return;
            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.SetupButton(i);
        }
    }

    private bool IsLevelUncloked(int levelIndex) => levelsUnlocked[levelIndex];

    private void LoadLevelsInfo()
    {
        var levelsAmount = SceneManager.sceneCountInBuildSettings - 1;
        
        levelsUnlocked = new bool[levelsAmount];

        for (var i = 1; i < levelsAmount; i++)
        {
            var levelUnlocked = PlayerPrefs.GetInt("Level" + i + "Unlocked", 0) == 1;
            
            if (levelUnlocked)
                levelsUnlocked[i] = true;
        }

        levelsUnlocked[1] = true;
    }
}
