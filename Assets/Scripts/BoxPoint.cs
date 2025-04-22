using UnityEngine;

public class BoxPoint : MonoBehaviour
{
    public Color requiredColor = Color.white;
    public bool isOccupied = false;
    private float centerTolerance = 0.2f;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Box box = other.GetComponent<Box>();
            if (box != null && box.boxColor.Equals(requiredColor))
            {
                // Check if box is centered on point using distance
                float distance = Vector2.Distance(transform.position, other.transform.position);
                
                if (distance <= centerTolerance)
                {
                    if (!isOccupied)
                    {
                        isOccupied = true;
                        AudioManager.instance.PlaySFX(3);
                        Debug.Log("Box landed on BoxPoint");
                        
                        if (GameManager.instance != null)
                        {
                            GameManager.instance.UpdateBoxCount();
                            GameManager.instance.CheckLevelCompletion();
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Box box = other.GetComponent<Box>();
            if (box != null && box.boxColor.Equals(requiredColor) && isOccupied)
            {
                isOccupied = false;
                if (GameManager.instance != null)
                    GameManager.instance.UpdateBoxCount();
            }
        }
    }
}