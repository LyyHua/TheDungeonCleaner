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
    [SerializeField] private LayerMask boxCollisionLayer; // assign the Box layer here

    private bool isMoving;
    private Vector3 origPos;
    private float xInput;

    private void Update()
    {
        if (isMoving) return;
        
        HandleInput();
        HandleFlip();
    }
    
    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        
        // Combine wall and box collision checks
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
        
        // Check for collisions with both wall and box layer
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
        RaycastHit2D hitBox = Physics2D.Raycast(transform.position, direction, 1f, boxCollisionLayer);
        
        bool canMove = (hitWall.collider == null && hitBox.collider == null);

        if (canMove)
        {
            yield return PerformMove(origPos, origPos + direction, timeToMove);
        }
        else
        {
            // Bounce logic if hitting wall or box
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
}