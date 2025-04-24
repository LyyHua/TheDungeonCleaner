using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float timeToMove = 0.15f;
    [SerializeField] private float bounceDistance = 0.25f;
    [SerializeField] private float bounceTime = 0.05f;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxCollisionLayer;
    
    [Header("Player Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] availableSkins;
    [SerializeField] private ParticleSystem dustFx;

    private bool isMoving;
    private Vector3 origPos;
    private float xInput;

    private PlayerBoxInteraction playerBoxInteraction;

    private void Awake()
    {
        playerBoxInteraction = GetComponent<PlayerBoxInteraction>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ApplySkin();
    }

    private void Update()
    {
        if (isMoving) return;

        HandleInput();
        HandleFlip();
    }
    
    private void ApplySkin()
    {
        if (SkinManager.instance == null || spriteRenderer == null || availableSkins == null || availableSkins.Length == 0)
            return;

        int skinId = SkinManager.instance.GetSkinId();

        if (skinId >= 0 && skinId < availableSkins.Length)
            spriteRenderer.sprite = availableSkins[skinId];
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.W) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.up));
        if (Input.GetKey(KeyCode.A) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.left));
        if (Input.GetKey(KeyCode.S) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.down));
        if (Input.GetKey(KeyCode.D) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.right));
    }
    
    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        origPos = transform.position;
        playerBoxInteraction.SaveState(null, null);

        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
        RaycastHit2D hitBox = Physics2D.Raycast(transform.position, direction, 1f, boxCollisionLayer);

        bool canMove = (hitWall.collider == null && hitBox.collider == null);

        if (canMove)
        {
            AudioManager.instance.PlaySFX(1);
            dustFx.Play();
            yield return PerformMove(origPos, origPos + direction, timeToMove);
        }
        else
        {
            AudioManager.instance.PlaySFX(0);
            yield return PerformBounce(origPos, direction);
        }
        isMoving = false;
    }

    private IEnumerator PerformMove(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    private IEnumerator PerformBounce(Vector3 startPos, Vector3 direction)
    {
        Vector3 bouncePos = startPos + direction * bounceDistance;
        yield return PerformMove(startPos, bouncePos, bounceTime);
        yield return PerformMove(bouncePos, startPos, bounceTime);
    }

    private void HandleFlip()
    {
        if (xInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (xInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
    
    public void ResetState()
    {
        StopAllCoroutines();
        isMoving = false;
        enabled = true;
    }
    
    public void PlayDustEffect() => dustFx.Play();
}