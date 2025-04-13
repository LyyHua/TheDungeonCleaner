using UnityEngine;
using UnityEngine.UI;

public class UI_SkinSelection : MonoBehaviour
{
    [Header("Skin Navigation")]
    [SerializeField] private int currentIndex;
    [SerializeField] private Text skinNameText;

    [Header("UI Preview")]
    [SerializeField] private SpriteRenderer previewSpriteRenderer;
    [SerializeField] private Sprite[] skinSprites;

    [Header("Character Preview")]
    [SerializeField] private GameObject menuCharacter;

    private void Start()
    {
        if (SkinManager.instance != null)
        {
            currentIndex = SkinManager.instance.GetSkinId();
        }
        UpdateSkinDisplay();
    }

    public void SelectSkin()
    {
        if (SkinManager.instance != null)
        {
            SkinManager.instance.SetSkinId(currentIndex);
        }
    }

    public void NextSkin()
    {
        currentIndex++;
        if (currentIndex >= skinSprites.Length)
            currentIndex = 0;

        UpdateSkinDisplay();
    }

    public void PreviousSkin()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = skinSprites.Length - 1;

        UpdateSkinDisplay();
    }

    private void UpdateSkinDisplay()
    {
        // Update UI preview sprite
        if (previewSpriteRenderer != null && currentIndex >= 0 && currentIndex < skinSprites.Length)
        {
            previewSpriteRenderer.sprite = skinSprites[currentIndex];
        }

        // Update skin name text
        if (skinNameText != null)
        {
            skinNameText.text = $"Skin {currentIndex + 1}";
        }

        // Update character model
        if (menuCharacter != null)
        {
            var characterSkin = menuCharacter.GetComponent<MenuCharacterSkin>();
            if (characterSkin != null)
            {
                characterSkin.PreviewSkin(currentIndex);
            }
        }
    }
}