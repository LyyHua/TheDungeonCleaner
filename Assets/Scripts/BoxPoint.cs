using UnityEngine;

public class BoxPoint : MonoBehaviour
{
    public Color requiredColor = Color.white;
    public bool isOccupied = false;
    private float centerTolerance = 0.2f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckAndActivateBoxPoint(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        CheckAndActivateBoxPoint(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Box")) return;
        
        Box box = other.GetComponent<Box>();
        if (box == null || !box.boxColor.Equals(requiredColor) || !isOccupied) return;
        
        isOccupied = false;
        box.SetOnPointState(false);
        if (GameManager.instance != null)
            GameManager.instance.UpdateBoxCount();
    }
    
    private void CheckAndActivateBoxPoint(Collider2D other)
    {
        if (!other.CompareTag("Box")) return;

        Box box = other.GetComponent<Box>();
        if (box == null || !box.boxColor.Equals(requiredColor)) return;

        float distance = Vector2.Distance(transform.position, other.transform.position);
        if (!(distance <= centerTolerance)) return;
    
        // Always update the visual state immediately even during transitions
        box.SetOnPointState(true);
    
        // Only update occupation status if not already occupied
        if (!isOccupied)
        {
            isOccupied = true;
            AudioManager.instance.PlaySFX(3);
            GameManager.instance.UpdateBoxCount();
            GameManager.instance.CheckLevelCompletion();
        }
    }
}