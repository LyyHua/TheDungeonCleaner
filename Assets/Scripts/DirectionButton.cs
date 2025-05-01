using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector2 direction;
    private Player player;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        player.SetDirectionalInput(direction, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.SetDirectionalInput(Vector2.zero, false);
    }
    
}