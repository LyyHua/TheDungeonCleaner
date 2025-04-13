using UnityEngine;

public class MenuCharacterSkin : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] availableSkins;
    
    [SerializeField] 
    private int _currentSkinId = 0;
    
    public int currentSkinId
    {
        get => _currentSkinId;
        set
        {
            _currentSkinId = value;
            ApplySkinVisual(_currentSkinId);
        }
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        ApplySkinVisual(_currentSkinId);
    }

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (SkinManager.instance != null)
            _currentSkinId = SkinManager.instance.GetSkinId();

        ApplySkinVisual(_currentSkinId);
    }

    public void PreviewSkin(int skinIndex)
    {
        if (skinIndex < 0 || skinIndex >= availableSkins.Length || spriteRenderer == null)
            return;

        _currentSkinId = skinIndex;
        spriteRenderer.sprite = availableSkins[skinIndex];
    }

    private void ApplySkinVisual(int skinIndex)
    {
        if (spriteRenderer == null || availableSkins == null || availableSkins.Length == 0)
            return;
        
        if (skinIndex >= 0 && skinIndex < availableSkins.Length)
        {
            spriteRenderer.sprite = availableSkins[skinIndex];
        }
    }
    
    [ContextMenu("Update Skin Display")]
    public void UpdateSkinDisplay()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        ApplySkinVisual(_currentSkinId);
    }
}