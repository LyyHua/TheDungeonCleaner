using UnityEngine;
using UnityEngine.UI;

public class MenuCharacterSkin : MonoBehaviour
{
    [SerializeField] private Image imageRenderer;
    [SerializeField] private Sprite[] availableSkins;
    [SerializeField] private int currentSkinId = 0;

    private void OnValidate()
    {
        if (imageRenderer == null)
            imageRenderer = GetComponent<Image>();
            
        ApplySkinVisual(currentSkinId);
    }

    private void Start()
    {
        if (imageRenderer == null)
            imageRenderer = GetComponent<Image>();

        if (SkinManager.instance != null)
            currentSkinId = SkinManager.instance.GetSkinId();

        ApplySkinVisual(currentSkinId);
    }

    public void PreviewSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= availableSkins.Length || imageRenderer == null)
            return;

        currentSkinId = skinIndex;
        imageRenderer.sprite = availableSkins[skinIndex];
    }

    private void ApplySkinVisual(int skinIndex)
    {
        if (imageRenderer == null || availableSkins == null || availableSkins.Length == 0)
            return;
        
        if (skinIndex >= 0 && skinIndex < availableSkins.Length)
            imageRenderer.sprite = availableSkins[skinIndex];
    }
    
    [ContextMenu("Update Skin Display")]
    public void UpdateSkinDisplay()
    {
        if (imageRenderer == null)
            imageRenderer = GetComponent<Image>();
            
        ApplySkinVisual(currentSkinId);
    }
}