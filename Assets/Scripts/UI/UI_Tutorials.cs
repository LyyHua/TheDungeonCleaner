using System.Collections;
using UnityEngine;

public class UI_Tutorials : MonoBehaviour
{
    [SerializeField] private GameObject firstTimeTutorial;
    [SerializeField] private GameObject guide;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player != null)
        {
            firstTimeTutorial.SetActive(false);
            StartCoroutine(ActivateGuideAfterDelay(2f));
        }
    }
    
    private IEnumerator ActivateGuideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        guide.SetActive(true);
    }
}
