using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DirectionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector2 direction;
    private Player player;
    private bool isPointerDown = false;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        if (player != null)
            player.SetDirectionalInput(direction, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        if (player != null)
            player.SetDirectionalInput(Vector2.zero, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If finger is already down and slides onto this button
        if (isPointerDown && player != null)
            player.SetDirectionalInput(direction, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Only clear input if we're not sliding to another direction button
        // We need to check if it went to another button
        if (isPointerDown && player != null)
        {
            // We're going to let the button being entered handle setting the new direction
            // But we need to clear our direction if no other button is immediately entered
            if (!IsPointerOverDirectionButton(eventData))
            {
                player.SetDirectionalInput(Vector2.zero, false);
            }
        }
    }

    private bool IsPointerOverDirectionButton(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            // Check if the object has a DirectionButton component
            return eventData.pointerCurrentRaycast.gameObject.GetComponent<DirectionButton>() != null;
        }
        return false;
    }
    
    public static void ResetAllDirectionInputs()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetDirectionalInput(Vector2.zero, false);
    }
}