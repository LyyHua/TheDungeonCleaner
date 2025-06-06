using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiddleButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color normalColor = Color.white;

    private Player player;
    private Image buttonImage;
    private bool isPointerDown = false;
    private RectTransform rectTransform;
    private Vector2 lastDirection = Vector2.zero;

    // Reference to surrounding direction buttons for visual feedback
    [SerializeField] private DirectionButton upButton;
    [SerializeField] private DirectionButton downButton;
    [SerializeField] private DirectionButton leftButton;
    [SerializeField] private DirectionButton rightButton;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
            buttonImage = GetComponentInChildren<Image>();

        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        Vector2 direction = CalculateDirectionFromPosition(eventData.position);
        lastDirection = direction;

        // Set appropriate visual feedback based on quadrant
        UpdateVisualFeedback(direction);

        player?.SetDirectionalInput(direction, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isPointerDown) return;

        Vector2 direction = CalculateDirectionFromPosition(eventData.position);
        
        // Only update if direction changed
        if (direction != lastDirection)
        {
            lastDirection = direction;
            UpdateVisualFeedback(direction);
            player?.SetDirectionalInput(direction, true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        lastDirection = Vector2.zero;
        ResetVisualFeedback();
        player?.SetDirectionalInput(Vector2.zero, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.currentInputModule.input.GetMouseButton(0))
        {
            isPointerDown = true;
            Vector2 direction = CalculateDirectionFromPosition(eventData.position);
            lastDirection = direction;

            UpdateVisualFeedback(direction);
            player?.SetDirectionalInput(direction, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPointerDown && player != null)
        {
            if (!IsPointerOverDirectionButton(eventData))
            {
                ResetVisualFeedback();
                player.SetDirectionalInput(Vector2.zero, false);
            }
        }
    }

    private Vector2 CalculateDirectionFromPosition(Vector2 pointerPosition)
    {
        // Convert screen position to local position within the button
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerPosition, null, out localPoint);

        // For diagonal divisions (X-shaped):
        // Compare y with x and -x to determine the quadrant
        if (localPoint.y > localPoint.x && localPoint.y > -localPoint.x)
            return Vector2.up;    // Top quadrant
        else if (localPoint.x > localPoint.y && localPoint.x > -localPoint.y)
            return Vector2.right; // Right quadrant
        else if (localPoint.y < localPoint.x && localPoint.y < -localPoint.x)
            return Vector2.down;  // Bottom quadrant
        else
            return Vector2.left;  // Left quadrant
    }

    private void UpdateVisualFeedback(Vector2 direction)
    {
        // Reset all buttons to normal state
        if (upButton != null) upButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
        if (downButton != null) downButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
        if (leftButton != null) leftButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
        if (rightButton != null) rightButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);

        // Highlight only the appropriate button based on quadrant
        if (direction == Vector2.up && upButton != null)
            upButton.SendMessage("SetPressedVisual", true, SendMessageOptions.DontRequireReceiver);
        else if (direction == Vector2.down && downButton != null)
            downButton.SendMessage("SetPressedVisual", true, SendMessageOptions.DontRequireReceiver);
        else if (direction == Vector2.left && leftButton != null)
            leftButton.SendMessage("SetPressedVisual", true, SendMessageOptions.DontRequireReceiver);
        else if (direction == Vector2.right && rightButton != null)
            rightButton.SendMessage("SetPressedVisual", true, SendMessageOptions.DontRequireReceiver);

        buttonImage.color = pressedColor;
    }

    private void ResetVisualFeedback()
    {
        buttonImage.color = normalColor;

        if (upButton != null) upButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
        if (downButton != null) downButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
        if (leftButton != null) leftButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
        if (rightButton != null) rightButton.SendMessage("SetPressedVisual", false, SendMessageOptions.DontRequireReceiver);
    }

    private bool IsPointerOverDirectionButton(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
            return eventData.pointerCurrentRaycast.gameObject.GetComponent<DirectionButton>() != null;

        return false;
    }
}