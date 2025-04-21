using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBoxInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode grabToggleKey = KeyCode.E;
    [SerializeField] private KeyCode undoKey = KeyCode.U;
    [SerializeField] private KeyCode resetKey = KeyCode.R;
    [SerializeField] private float detectionDistance = 0.6f;
    [SerializeField] private float detectionRadius = 0.3f;
    [SerializeField] private LayerMask boxLayer;

    [Header("Grid Movement Settings")]
    [SerializeField] private float gridMoveDuration = 0.15f;
    [SerializeField] private float bounceDistance = 0.25f;
    [SerializeField] private LayerMask wallLayer;

    private Box currentBox;
    private Box highlightedBox;
    private bool isDragging = false;
    private Vector2 lastInputDirection = Vector2.right;
    private Vector2 grabDirection = Vector2.zero;
    private bool isGridMoving = false;

    private Transform playerTransform;
    private Player playerComponent;

    // Undo stack to track individual movements
    private Stack<(Vector3 playerPos, Vector2 facingDirection, Box box, Vector3? boxPos)> undoStack = new();

    private void Awake()
    {
        playerTransform = transform;
        playerComponent = GetComponent<Player>();
    }

    private void Start()
    {
        SaveState(null, null); // Save the initial state
    }

    private void Update()
    {
        if (!isDragging && !isGridMoving)
        {
            Vector2 input = Vector2.zero;
            if (Input.GetKey(KeyCode.W)) input.y = 1;
            else if (Input.GetKey(KeyCode.S)) input.y = -1;
            if (Input.GetKey(KeyCode.D)) input.x = 1;
            else if (Input.GetKey(KeyCode.A)) input.x = -1;

            if (input != Vector2.zero)
                lastInputDirection = input;

            HighlightGrabbableBox();
        }

        if (Input.GetKeyDown(grabToggleKey) && !isGridMoving)
        {
            if (!isDragging)
                TryGrabBox();
            else
                ReleaseBox();
        }

        if (Input.GetKeyDown(undoKey) && !isGridMoving)
        {
            UndoMove();
        }

        if (Input.GetKeyDown(resetKey))
        {
            ResetLevel();
        }

        // Allow continuous drag movement by holding a key.
        if (isDragging && !isGridMoving)
        {
            Vector2 allowedDir = -grabDirection;
            if (allowedDir == Vector2.up && Input.GetKeyDown(KeyCode.W))
                StartCoroutine(DragMove(Vector3.up));
            else if (allowedDir == Vector2.down && Input.GetKeyDown(KeyCode.S))
                StartCoroutine(DragMove(Vector3.down));
            else if (allowedDir == Vector2.left && Input.GetKeyDown(KeyCode.A))
                StartCoroutine(DragMove(Vector3.left));
            else if (allowedDir == Vector2.right && Input.GetKeyDown(KeyCode.D))
                StartCoroutine(DragMove(Vector3.right));
        }
    }

    private void HighlightGrabbableBox()
    {
        Vector3 detectPoint = playerTransform.position + (Vector3)(lastInputDirection.normalized * detectionDistance);
        Collider2D hit = Physics2D.OverlapCircle(detectPoint, detectionRadius, boxLayer);

        if (hit != null && hit.CompareTag("Box"))
        {
            Box box = hit.GetComponent<Box>();
            if (box != null && box != highlightedBox)
            {
                if (highlightedBox != null)
                    highlightedBox.SetOutlineColor(false);

                highlightedBox = box;
                highlightedBox.SetOutlineColor(true);
            }
        }
        else if (highlightedBox != null)
        {
            highlightedBox.SetOutlineColor(false);
            highlightedBox = null;
        }
    }

    private void TryGrabBox()
    {
        if (lastInputDirection == Vector2.zero)
        {
            Debug.Log("No stored directional key; cannot grab.");
            return;
        }

        Vector3 detectPoint = playerTransform.position + (Vector3)(lastInputDirection.normalized * detectionDistance);
        Collider2D hit = Physics2D.OverlapCircle(detectPoint, detectionRadius, boxLayer);
        if (hit != null && hit.CompareTag("Box"))
        {
            currentBox = hit.GetComponent<Box>();
            grabDirection = lastInputDirection.normalized;
            isDragging = true;

            currentBox.SetOutlineColor(false);

            if (playerComponent)
                playerComponent.enabled = false;

            SaveState(currentBox, null);
        }
    }
    
    private IEnumerator DragMove(Vector3 direction)
    {
        isGridMoving = true;
        Vector3 startPosPlayer = playerTransform.position;
        Vector3 startPosBox = currentBox.transform.position;
        Vector3 endPosPlayer = startPosPlayer + direction;
        Vector3 endPosBox = startPosBox + direction;

        // Check for player collisions with walls
        bool playerBlockedByWall = Physics2D.OverlapCircle(endPosPlayer, 0.1f, wallLayer);

        // Check if the player's path is blocked by another box
        bool playerBlockedByBox = false;
        Collider2D[] playerPathColliders = Physics2D.OverlapCircleAll(endPosPlayer, 0.1f, boxLayer);
        foreach (var col in playerPathColliders)
        {
            // If we detect any box other than the one we're dragging
            if (col.gameObject != currentBox.gameObject)
            {
                playerBlockedByBox = true;
                break;
            }
        }

        // Check if the path is blocked
        bool isBlocked = playerBlockedByWall || playerBlockedByBox;
        if (isBlocked)
        {
            Vector3 bouncePos = startPosPlayer + direction * bounceDistance;
            yield return MoveBoth(startPosPlayer, bouncePos, gridMoveDuration * 0.33f);
            yield return MoveBoth(bouncePos, startPosPlayer, gridMoveDuration * 0.33f);
        }
        else
        {
            // Save state before moving
            SaveState(currentBox, startPosBox);
            yield return MoveBoth(startPosPlayer, endPosPlayer, gridMoveDuration);
        }

        isGridMoving = false;
    }

    private IEnumerator MoveBoth(Vector3 startPlayer, Vector3 endPlayer, float duration)
    {
        float elapsed = 0f;
        Vector3 startBox = currentBox.transform.position;
        Vector3 endBox = startBox + (endPlayer - startPlayer);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playerTransform.position = Vector3.Lerp(startPlayer, endPlayer, t);
            currentBox.transform.position = Vector3.Lerp(startBox, endBox, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        playerTransform.position = endPlayer;
        currentBox.transform.position = endBox;
    }

    private void ReleaseBox()
    {
        if (currentBox != null)
        {
            SaveState(currentBox, currentBox.transform.position);
        }
        isDragging = false;
        if (playerComponent)
            playerComponent.enabled = true;

        currentBox = null;
        grabDirection = Vector2.zero;
    }

    public void SaveState(Box box, Vector3? boxPos)
    {
        undoStack.Push((playerTransform.position, lastInputDirection, box, boxPos));
    }

    public void UndoMove()
    {
        if (undoStack.Count > 0)
        {
            var (playerPos, facingDirection, box, boxPos) = undoStack.Pop();
            playerTransform.position = playerPos;
            lastInputDirection = facingDirection;
            if (lastInputDirection.x < 0)
                playerTransform.localScale = new Vector3(-1, 1, 1);
            else if (lastInputDirection.x > 0)
                playerTransform.localScale = new Vector3(1, 1, 1);
            if (box != null && boxPos.HasValue)
            {
                box.transform.position = boxPos.Value;
            }
        }
        else
        {
            Debug.Log("No more states to undo.");
        }
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}