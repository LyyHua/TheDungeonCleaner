using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float timeToMove = 0.15f;
    [SerializeField] private float bounceDistance = 0.25f;
    [SerializeField] private float bounceTime = 0.05f;
    [SerializeField] private float bufferMoveWindow = 0.075f;
    private float bufferMoveActivated = -1f;
    private Vector2 bufferedDirection = Vector2.zero;

    [Header("Collision")]
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxCollisionLayer;

    [Header("Player Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] availableSkins;
    [SerializeField] private ParticleSystem dustFx;

    private bool isMoving;
    private Vector2 origPos;
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
        if (isMoving)
            AttemptBufferMove();
        else
            HandleInput();

        HandleFlip();
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            bufferMoveActivated = Time.time;
            StartCoroutine(MovePlayer(Vector2.up));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            bufferMoveActivated = Time.time;
            StartCoroutine(MovePlayer(Vector2.left));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            bufferMoveActivated = Time.time;
            StartCoroutine(MovePlayer(Vector2.down));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            bufferMoveActivated = Time.time;
            StartCoroutine(MovePlayer(Vector2.right));
        }
    }

    private void AttemptBufferMove()
    {
        if (bufferedDirection != Vector2.zero && Time.time < bufferMoveActivated + bufferMoveWindow)
        {
            Vector2 moveDir = bufferedDirection;
            bufferedDirection = Vector2.zero;
            StartCoroutine(MovePlayer(moveDir));
        }
    }

    // Movement using Vector2 for calculation and converting to Vector3 when updating position.
    private IEnumerator MovePlayer(Vector2 direction)
    {
        isMoving = true;
        // Convert transform.position to Vector2
        origPos = new Vector2(transform.position.x, transform.position.y);
        playerBoxInteraction.SaveState(null, null);

        // Calculate the target position in 2D then convert back for transform.position (using z from current transform)
        Vector2 targetPos = origPos + direction;
        Vector3 targetPosition = new Vector3(targetPos.x, targetPos.y, transform.position.z);

        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
        RaycastHit2D hitBox = Physics2D.Raycast(transform.position, direction, 1f, boxCollisionLayer);
        bool canMove = (hitWall.collider == null && hitBox.collider == null);

        if (canMove)
        {
            AudioManager.instance.PlaySFX(1);
            dustFx.Play();
            yield return PerformMove(transform.position, targetPosition, timeToMove);
        }
        else
        {
            AudioManager.instance.PlaySFX(0);
            yield return PerformBounce(transform.position, direction);
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

    private IEnumerator PerformBounce(Vector3 startPos, Vector2 direction)
    {
        Vector2 bouncePos2D = new Vector2(startPos.x, startPos.y) + direction * bounceDistance;
        Vector3 bouncePos = new Vector3(bouncePos2D.x, bouncePos2D.y, startPos.z);
        yield return PerformMove(startPos, bouncePos, bounceTime);
        yield return PerformMove(bouncePos, startPos, bounceTime);
    }

    private void HandleFlip()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        if (xInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (xInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }

    public void ResetState()
    {
        StopAllCoroutines();
        isMoving = false;
        bufferedDirection = Vector2.zero;
        enabled = true;
    }

    public void PlayDustEffect() => dustFx.Play();

    private void ApplySkin()
    {
        if (SkinManager.instance == null || spriteRenderer == null || availableSkins == null || availableSkins.Length == 0)
            return;
        int skinId = SkinManager.instance.GetSkinId();
        if (skinId >= 0 && skinId < availableSkins.Length)
            spriteRenderer.sprite = availableSkins[skinId];
    }
}