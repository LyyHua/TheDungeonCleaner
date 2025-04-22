using System;
using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    public bool isOccupied = false;
    private float tolerance = 0.1f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Vector2.Distance(transform.position, other.transform.position) <= tolerance)
            {
                if (!isOccupied)
                {
                    isOccupied = true;
                    AudioManager.instance.PlaySFX(3);
                    if (GameManager.instance != null)
                    {
                        GameManager.instance.CheckLevelCompletion();
                        GameManager.instance.UpdatePlayerCount();
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isOccupied)
            {
                isOccupied = false;
                if (GameManager.instance != null)
                    GameManager.instance.UpdatePlayerCount();
            }
        }
    }
}