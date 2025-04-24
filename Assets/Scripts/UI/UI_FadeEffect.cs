using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeEffect : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    
    private void Start()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
    }

    public void ScreenFade(float targetAlpha, float duration, System.Action onComplete = null)
    {
        StartCoroutine(FadeCoroutine(targetAlpha, duration, onComplete));
    }

    private IEnumerator FadeCoroutine(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0;
        var currentColor = fadeImage.color;
        var startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            
            var alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            
            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            
            yield return null;
        }
        
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        
        onComplete?.Invoke();
    }
}
