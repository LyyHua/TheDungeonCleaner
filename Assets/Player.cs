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
    
    private bool isMoving;
    private Vector3 origPos, targetPos;
    private bool facingRight = true;
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
        
        if(Input.GetKey(KeyCode.W) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.up));
        if(Input.GetKey(KeyCode.A) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.left));
        if(Input.GetKey(KeyCode.S) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.down));
        if(Input.GetKey(KeyCode.D) && !isMoving)
            StartCoroutine(MovePlayer(Vector3.right));
    }

    private void HandleFlip()
    {
        if (xInput < 0 && facingRight || xInput > 0 && !facingRight)
            Flip();
    }
    
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }
    
    private IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        origPos = transform.position;

        // Check if movement is valid
        var hit = Physics2D.Raycast(transform.position, direction, 1f, wallLayer);
        var canMove = !hit.collider;

        yield return canMove ? 
            PerformMove(origPos, origPos + direction, timeToMove) : 
            PerformBounce(origPos, direction);

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
        var bouncePos = startPos + direction * bounceDistance;
        
        // Move toward the wall
        yield return PerformMove(startPos, bouncePos, bounceTime);
        
        // Move back
        yield return PerformMove(bouncePos, startPos, bounceTime);
    }
}