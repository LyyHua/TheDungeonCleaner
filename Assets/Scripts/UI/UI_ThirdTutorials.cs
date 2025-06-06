using UnityEngine;

public class UI_ThirdTutorials : MonoBehaviour
{
    [SerializeField] private GameObject thirdTutorial;
    [SerializeField] private GameObject fourthTutorial;
    [SerializeField] private GameObject grabButton;
    [SerializeField] private GameObject movingPad;
    [SerializeField] private GameObject fifthTutorial;
    [SerializeField] private GameObject finalTutorial;
    [SerializeField] private GameObject point;
    
    public void ChangeToFourthTutorial()
    {
        if (thirdTutorial.activeSelf)
        {
            thirdTutorial.SetActive(false);
            fourthTutorial.SetActive(true);
            grabButton.SetActive(false);
            movingPad.SetActive(true);
            point.SetActive(true);
        }
        else
        {
            grabButton.SetActive(false);
            movingPad.SetActive(true);
            fifthTutorial.SetActive(false);
            finalTutorial.SetActive(true);
        }
    }
}
