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
        
        if (skinControllers.Length == 0) return;
        
        foreach (var controller in skinControllers) controller.PreviewSkin(choosenSkinId);
    }
}