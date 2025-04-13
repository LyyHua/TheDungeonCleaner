using System;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public int choosenSkinId;
    public static SkinManager instance;
    
    private const string SKIN_PREFS_KEY = "SelectedSkinID";

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load saved skin ID if available
            if (PlayerPrefs.HasKey(SKIN_PREFS_KEY))
            {
                choosenSkinId = PlayerPrefs.GetInt(SKIN_PREFS_KEY);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetSkinId(int id)
    {
        choosenSkinId = id;
        
        // Save the selected skin ID
        PlayerPrefs.SetInt(SKIN_PREFS_KEY, id);
        PlayerPrefs.Save();
    }
    
    public int GetSkinId() => choosenSkinId;
}
