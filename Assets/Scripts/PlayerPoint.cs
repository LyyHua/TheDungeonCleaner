using System;
using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    public bool isOccupied = false;
    private float tolerance = 0.1f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
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
        if (!isOccupied) return;
        isOccupied = false;
        if (GameManager.instance != null)
            GameManager.instance.UpdatePlayerCount();
    }
}