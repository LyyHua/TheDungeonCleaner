using UnityEngine;

public class Box : MonoBehaviour
{
    public Color boxColor = Color.white;

    [Header("Sprites")]
    [SerializeField] private Sprite blackOutlineSprite;
    [SerializeField] private Sprite whiteOutlineSprite;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = blackOutlineSprite;
    }

    public void SetOutlineColor(bool isWhite)
    {
        if (spriteRenderer == null) return;

        // Switch between black and white outline sprites
        spriteRenderer.sprite = isWhite ? whiteOutlineSprite : blackOutlineSprite;
    }
}
