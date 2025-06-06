using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DirectionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private bool isCenter = false;
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color normalColor = Color.white;
    
    private Player player;
    private Image buttonImage;
    private bool isPointerDown = false;
    private static Vector2 lastActiveDirection = Vector2.zero;
    private static DirectionButton currentActiveButton = null;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
            buttonImage = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
        if (isCenter)
            direction = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        SetActiveButton(this);

        if (isCenter && lastActiveDirection != Vector2.zero)
        {
            player?.SetDirectionalInput(lastActiveDirection, true);
        }
        else
        {
            if (!isCenter && direction != Vector2.zero)
                lastActiveDirection = direction;
                
            player?.SetDirectionalInput(direction, true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        ResetActiveButton();
        player?.SetDirectionalInput(Vector2.zero, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPointerDown || EventSystem.current.currentInputModule.input.GetMouseButton(0))
        {
            SetActiveButton(this);
            
            if (isCenter && lastActiveDirection != Vector2.zero)
            {
                player?.SetDirectionalInput(lastActiveDirection, true);
            }
            else
            {
                if (!isCenter && direction != Vector2.zero)
                    lastActiveDirection = direction;
                    
                player?.SetDirectionalInput(direction, true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPointerDown && player != null)
        {
            if (!IsPointerOverDirectionButton(eventData))
            {
                ResetActiveButton();
                player.SetDirectionalInput(Vector2.zero, false);
            }
        }
    }

    private bool IsPointerOverDirectionButton(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
            return eventData.pointerCurrentRaycast.gameObject.GetComponent<DirectionButton>() != null;

        return false;
    }

    private static void SetActiveButton(DirectionButton button)
    {
        // Reset previous button if any
        if (currentActiveButton != null && currentActiveButton != button)
        {
            currentActiveButton.SetPressedVisual(false);
        }
        
        // Set new active button
        currentActiveButton = button;
        button.SetPressedVisual(true);
    }

    private static void ResetActiveButton()
    {
        if (currentActiveButton != null)
        {
            currentActiveButton.SetPressedVisual(false);
            currentActiveButton = null;
        }
    }

    private void SetPressedVisual(bool isPressed)
    {
        if (buttonImage != null)
        {
            buttonImage.color = isPressed ? pressedColor : normalColor;
        }
    }

    public static void ResetAllDirectionInputs()
    {
        lastActiveDirection = Vector2.zero;
        ResetActiveButton();
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
            player.SetDirectionalInput(Vector2.zero, false);
    }
}