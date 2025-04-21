using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    private void Start()
    {
        CreateLevelButton();
    }

    private void CreateLevelButton()
    {
        var levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        // Start from 1 to skip the main menu scene
        for (int i = 1; i < levelsAmount; i++)
        {
            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.SetupButton(i);
        }
    }
}
