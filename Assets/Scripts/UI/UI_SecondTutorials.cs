using UnityEngine;

public class UI_SecondTutorials : MonoBehaviour
{
    [SerializeField] private GameObject thirdTutorial;
    [SerializeField] private GameObject secondTutorial;
    [SerializeField] private GameObject movingPad;
    [SerializeField] private GameObject grabButton;
    [SerializeField] private GameObject thePoint;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.SetDirectionalInput(Vector2.zero, false);
            secondTutorial.SetActive(false);
            thirdTutorial.SetActive(true);
            movingPad.SetActive(false);
            grabButton.SetActive(true);
            thePoint.SetActive(false);
        }
    }
}
