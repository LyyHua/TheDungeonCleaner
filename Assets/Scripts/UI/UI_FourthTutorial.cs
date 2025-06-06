using UnityEngine;

public class UI_FourthTutorial : MonoBehaviour
{
    [SerializeField] private GameObject fourthTutorial;
    [SerializeField] private GameObject fifthTutorial;
    [SerializeField] private GameObject movePad;
    [SerializeField] private GameObject releaseButton;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.SetDirectionalInput(Vector2.zero, false);
            fourthTutorial.SetActive(false);
            fifthTutorial.SetActive(true);
            movePad.SetActive(false);
            releaseButton.SetActive(true);
        }
    }
}
