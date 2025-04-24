using UnityEngine;

public class Box : MonoBehaviour
{
    public Color boxColor = Color.white;

    [Header("Box Visuals")]
    [SerializeField] private Sprite blackOutlineSprite;
    [SerializeField] private Sprite whiteOutlineSprite;
    [SerializeField] private ParticleSystem dustFx;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = blackOutlineSprite;
    }

    public void SetOutlineColor(bool isWhite) => spriteRenderer.sprite = isWhite ? whiteOutlineSprite : blackOutlineSprite;
    
    public void PlayDustEffect() => dustFx.Play();
}
