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
                    Debug.Log("Player landed on PlayerPoint.");
                }
            }
            else if (isOccupied)
            {
                isOccupied = false;
                Debug.Log("Player moved off PlayerPoint.");
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
                Debug.Log("Player left PlayerPoint.");
            }
        }
    }
}