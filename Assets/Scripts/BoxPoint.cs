using UnityEngine;

public class BoxPoint : MonoBehaviour
{
    public Color requiredColor = Color.white;
    public bool isOccupied = false;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Box box = other.GetComponent<Box>();
            if (box != null && box.boxColor.Equals(requiredColor))
            {
                Vector2 gridPos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
                Vector2 boxGridPos = new Vector2(Mathf.Round(other.transform.position.x), Mathf.Round(other.transform.position.y));
                
                if (gridPos == boxGridPos)
                {
                    if (!isOccupied)
                    {
                        isOccupied = true;
                        Debug.Log("Box landed on BoxPoint at grid position: " + gridPos);
                    }
                }
                else if (isOccupied)
                {
                    isOccupied = false;
                    Debug.Log("Box moved off BoxPoint at grid position: " + gridPos);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Box box = other.GetComponent<Box>();
            if (box != null && box.boxColor.Equals(requiredColor))
            {
                if (isOccupied)
                {
                    isOccupied = false;
                    Debug.Log("Box left BoxPoint.");
                }
            }
        }
    }
}