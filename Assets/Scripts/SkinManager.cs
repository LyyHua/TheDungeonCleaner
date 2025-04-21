using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinManager : MonoBehaviour
{
    public int choosenSkinId;
    public static SkinManager instance;
    
    private const string SKIN_PREFS_KEY = "SelectedSkinID";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Load saved skin ID if available
            if (PlayerPrefs.HasKey(SKIN_PREFS_KEY))
            {
                choosenSkinId = PlayerPrefs.GetInt(SKIN_PREFS_KEY);
            }
            // Subscribe to scene loaded event to update player skin when scenes change
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update player skin when new scene loads
        UpdatePlayerSkin();
    }
    
    public void SetSkinId(int id)
    {
        choosenSkinId = id;
        
        // Save the selected skin ID
        PlayerPrefs.SetInt(SKIN_PREFS_KEY, id);
        PlayerPrefs.Save();
        
        // Update the player skin immediately
        UpdatePlayerSkin();
    }
    
    public int GetSkinId() => choosenSkinId;
    
    private void UpdatePlayerSkin()
    {
        // Find all MenuCharacterSkin instances in scene and update them
        MenuCharacterSkin[] skinControllers = FindObjectsByType<MenuCharacterSkin>(FindObjectsSortMode.None);
        
        if (skinControllers.Length == 0)
        {
            Debug.Log("No MenuCharacterSkin found in scene");
            return;
        }
        
        foreach (var controller in skinControllers)
        {
            controller.PreviewSkin(choosenSkinId);
            Debug.Log($"Updated skin to ID {choosenSkinId} on {controller.gameObject.name}");
        }
    }
}