using System.Collections;
using UnityEngine;

public class PlayerBoxInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private KeyCode grabToggleKey = KeyCode.E;
    [SerializeField] private float detectionDistance = 0.6f;
    [SerializeField] private float detectionRadius = 0.3f;
    [SerializeField] private LayerMask boxLayer;
    
    [Header("Grid Movement Settings")]
    [SerializeField] private float gridMoveDuration = 0.15f;
    [SerializeField] private float bounceDistance = 0.25f;
    [SerializeField] private LayerMask wallLayer;

    private Box currentBox;
    private bool isDragging = false;
    private Vector2 lastInputDirection = Vector2.zero;
    private Vector2 grabDirection = Vector2.zero;
    private bool isGridMoving = false;

    private Transform playerTransform;
    private Player playerComponent;

    private void Awake()
    {
        playerTransform = transform;
        playerComponent = GetComponent<Player>();
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
        }

        if (Input.GetKeyDown(grabToggleKey))
        {
            if (!isDragging)
                TryGrabBox();
            else
                ReleaseBox();
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
            if (playerComponent)
                playerComponent.enabled = false;
            Debug.Log("Box grabbed in direction: " + grabDirection);
        }
        else
        {
            Debug.Log("No box found in direction: " + lastInputDirection);
        }
    }

    private IEnumerator DragMove(Vector3 direction)
    {
        isGridMoving = true;
        Vector3 startPosPlayer = playerTransform.position;
        Vector3 startPosBox = currentBox.transform.position;
        Vector3 endPos = startPosPlayer + direction;

        // Check for wall collision from both player and box positions.
        RaycastHit2D hitPlayer = Physics2D.Raycast(startPosPlayer, direction, 1f, wallLayer);
        RaycastHit2D hitBox = Physics2D.Raycast(startPosBox, direction, 1f, wallLayer);

        if (hitPlayer.collider != null || hitBox.collider != null)
        {
            Vector3 bouncePos = startPosPlayer + direction * bounceDistance;
            yield return MoveBoth(startPosPlayer, bouncePos, gridMoveDuration * 0.33f);
            yield return MoveBoth(bouncePos, startPosPlayer, gridMoveDuration * 0.33f);
            Debug.Log("Bounce: blocked by wall while dragging");
        }
        else
        {
            yield return MoveBoth(startPosPlayer, endPos, gridMoveDuration);
            Debug.Log("Dragged grid step in direction: " + direction);
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
        isDragging = false;
        Debug.Log("Box released.");
        if (playerComponent)
            playerComponent.enabled = true;
        if (currentBox != null && GameManager.instance != null)
            GameManager.instance.CheckPuzzleCompletion(playerTransform, currentBox.transform);
        currentBox = null;
        grabDirection = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null)
            playerTransform = transform;
        Vector3 detectPoint = playerTransform.position + (Vector3)((lastInputDirection != Vector2.zero ? lastInputDirection : Vector2.up) * detectionDistance);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectPoint, detectionRadius);
    }
}