using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoFadeIntroUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup; // Drag CanvasGroup ind her
    [SerializeField] private float visibleDuration = 3f; // Hvor længe den er synlig
    [SerializeField] private float fadeDuration = 1.5f;   // Hvor lang tid det tager at fade ud

    private void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(visibleDuration);

        float t = 0f;
        float startAlpha = canvasGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            canvasGroup.alpha = a;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // Fjern Canvas fra visning
    }
}