using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float timeToMove = 0.135f;
    [SerializeField] private float bounceDistance = 0.25f;
    [SerializeField] private float bounceTime = 0.05f;
    [SerializeField] private float bufferMoveWindow = 0.065f;
    private float bufferMoveActivated = -1f;
    private Vector2 bufferedDirection = Vector2.zero;
    public Vector2 lastMovementDirection = Vector2.right;
    private bool isDraggingMode = false;

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
    private float yInput;
    private PlayerBoxInteraction playerBoxInteraction;
    private Joystick joyStick;

    private void Awake()
    {
        playerBoxInteraction = GetComponent<PlayerBoxInteraction>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        joyStick = FindFirstObjectByType<Joystick>();
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

    public Vector2 GetMovementDirection()
    {
        Vector2 input = Vector2.zero;
        
        // Mobile Input
        input = new Vector2(joyStick.Horizontal, joyStick.Vertical);
        
        // PC Input (temporarily disabled for mobile development)
        /*
        if (Input.GetKey(KeyCode.W))
            input = Vector2.up;
        else if (Input.GetKey(KeyCode.S))
            input = Vector2.down;
        else if (Input.GetKey(KeyCode.D))
            input = Vector2.right;
        else if (Input.GetKey(KeyCode.A))
            input = Vector2.left;
        */
        
        // Use a threshold before calculating dominant direction.
        if (!(Mathf.Abs(input.x) > 0.1f) && !(Mathf.Abs(input.y) > 0.1f)) return Vector2.zero;
        
        // Determine dominant direction.
        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            return (input.x > 0) ? Vector2.right : Vector2.left;
        return (input.y > 0) ? Vector2.up : Vector2.down;
    }

    private void HandleInput()
    {
        Vector2 direction = GetMovementDirection();
        if (direction == Vector2.zero) return;
        
        lastMovementDirection = direction;
        
        if (!isDraggingMode)
        {
            bufferMoveActivated = Time.time;
            StartCoroutine(MovePlayer(direction));
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
    
    private IEnumerator MovePlayer(Vector2 direction)
    {
        isMoving = true;
        origPos = new Vector2(transform.position.x, transform.position.y);
        playerBoxInteraction.SaveState(null, null);
        
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
        // xInput = Input.GetAxisRaw("Horizontal");
        // if (xInput < 0)
        //     transform.localScale = new Vector3(-1, 1, 1);
        // else if (xInput > 0)
        //     transform.localScale = new Vector3(1, 1, 1);
        // Get joystick input instead of keyboard only
        Vector2 input = joyStick.Direction;
    
        // Use the last movement direction if there's no current input
        if (input.magnitude < 0.1f)
        {
            // Flip based on last movement direction
            if (lastMovementDirection.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (lastMovementDirection.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }
        // Otherwise flip based on current input
        else if (Mathf.Abs(input.x) > 0.1f)
        {
            if (input.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (input.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }
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
    
    public void SetMovementSpeed(float newSpeed)
    {
        timeToMove = newSpeed;
    }
    
    public void SetDraggingMode(bool isDragging)
    {
        isDraggingMode = isDragging;
    }
}