using UnityEngine;

public class Box : MonoBehaviour
{
    public Color boxColor = Color.white;

    [Header("Sprites")]
    [SerializeField] private Sprite blackOutlineSprite; // Sprite with black outline
    [SerializeField] private Sprite whiteOutlineSprite; // Sprite with white outline

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on Box!");
        }

        // Set the default sprite to black outline
        spriteRenderer.sprite = blackOutlineSprite;
    }

    public void SetOutlineColor(bool isWhite)
    {
        if (spriteRenderer == null) return;

        // Switch between black and white outline sprites
        spriteRenderer.sprite = isWhite ? whiteOutlineSprite : blackOutlineSprite;
    }
}
