using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinManager : MonoBehaviour
{
    public int choosenSkinId;
    public static SkinManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            choosenSkinId = PlayerPrefs.GetInt("SelectedSkinID", 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetSkinId(int id)
    {
        choosenSkinId = id;
        
        PlayerPrefs.SetInt("SelectedSkinID", id);
        PlayerPrefs.Save();
        
        UpdatePlayerSkin();
    }
    
    public int GetSkinId() => choosenSkinId;
    
    private void UpdatePlayerSkin()
    {
        MenuCharacterSkin[] skinControllers = FindObjectsByType<MenuCharacterSkin>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
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