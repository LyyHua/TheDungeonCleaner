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
        if (!isDragging)
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

        if (Input.GetKeyDown(grabToggleKey))
        {
            if (!isDragging)
                TryGrabBox();
            else
                ReleaseBox();
        }

        if (Input.GetKeyDown(undoKey))
        {
            UndoMove();
        }

        if (Input.GetKeyDown(resetKey))
        {
            ResetLevel();
        }

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

            SaveState(currentBox, null); // Save the state when grabbing a box
        }
    }

    private IEnumerator DragMove(Vector3 direction)
    {
        isGridMoving = true;
        Vector3 startPosPlayer = playerTransform.position;
        Vector3 startPosBox = currentBox.transform.position;
        Vector3 endPos = startPosPlayer + direction;

        RaycastHit2D hitPlayer = Physics2D.Raycast(startPosPlayer, direction, 1f, wallLayer);
        RaycastHit2D hitBox = Physics2D.Raycast(startPosBox, direction, 1f, wallLayer);

        if (hitPlayer.collider != null || hitBox.collider != null)
        {
            Vector3 bouncePos = startPosPlayer + direction * bounceDistance;
            yield return MoveBoth(startPosPlayer, bouncePos, gridMoveDuration * 0.33f);
            yield return MoveBoth(bouncePos, startPosPlayer, gridMoveDuration * 0.33f);
        }
        else
        {
            SaveState(currentBox, startPosBox); // Save the state before moving the box
            yield return MoveBoth(startPosPlayer, endPos, gridMoveDuration);
        }

        isGridMoving = false;
    }

    private IEnumerator MoveBoth(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        Vector3 boxStart = currentBox.transform.position;
        Vector3 boxEnd = boxStart + (end - start);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playerTransform.position = Vector3.Lerp(start, end, t);
            currentBox.transform.position = Vector3.Lerp(boxStart, boxEnd, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerTransform.position = end;
        currentBox.transform.position = boxEnd;
    }

    private void ReleaseBox()
    {
        if (currentBox != null)
            SaveState(currentBox, currentBox.transform.position);

        isDragging = false;
        if (playerComponent)
            playerComponent.enabled = true;
    
        currentBox = null;
        grabDirection = Vector2.zero;
    }

    public void SaveState(Box box, Vector3? boxPos)
    {
        // Save the player's position and the box's position (if applicable)
        undoStack.Push((playerTransform.position, lastInputDirection, box, boxPos));
    }

    public void UndoMove()
    {
        if (undoStack.Count > 0)
        {
            // Pop the last state.
            var (playerPos, facingDirection, box, boxPos) = undoStack.Pop();
            // Revert player position.
            playerTransform.position = playerPos;
            // Update stored facing direction.
            lastInputDirection = facingDirection;
            // Update the player flip based on the restored facing direction.
            if (lastInputDirection.x < 0)
                playerTransform.localScale = new Vector3(-1, 1, 1);
            else if (lastInputDirection.x > 0)
                playerTransform.localScale = new Vector3(1, 1, 1);
        
            // Revert box position if a box and position were stored.
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