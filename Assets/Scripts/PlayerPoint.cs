using System;
using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    public bool isOccupied = false;
    private float tolerance = 0.1f;
    
    [SerializeField] private SpriteRenderer pointSpriteRenderer;
    [SerializeField] private Color activeColor = Color.green;
    private Color defaultColor = Color.white;
    
    private void Awake()
    {
        pointSpriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = pointSpriteRenderer.color;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // Change color to green when player enters
        if (pointSpriteRenderer != null)
            pointSpriteRenderer.color = activeColor;
        
        CheckPlayerOccupation(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        CheckPlayerOccupation(other);
    }
    
    private void CheckPlayerOccupation(Collider2D other)
    {
        if (!(Vector2.Distance(transform.position, other.transform.position) <= tolerance)) return;
        if (isOccupied) return;
        
        isOccupied = true;
        AudioManager.instance.PlaySFX(3);
        GameManager.instance.CheckLevelCompletion();
        GameManager.instance.UpdatePlayerCount();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // Change color back to default when player exits
        if (pointSpriteRenderer != null)
            pointSpriteRenderer.color = defaultColor;
        
        if (!isOccupied) return;
        isOccupied = false;
        if (GameManager.instance != null)
            GameManager.instance.UpdatePlayerCount();
    }
}