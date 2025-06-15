using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FishJourneyUI : MonoBehaviour
{
    public FishJourneyManager fishJourneyManager;
    public GameObject okButton;
    private CanvasGroup canvasGroup;

    public float fadeDuration = 0.5f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (okButton != null)
        {
            canvasGroup = okButton.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = okButton.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            okButton.SetActive(false);
        }
    }

    void Update()
    {
        if (fishJourneyManager == null || okButton == null)
            return;

        bool shouldShow = fishJourneyManager.IsWaitingForInput();

        if (shouldShow && !okButton.activeSelf)
        {
            okButton.SetActive(true);
            StartFade(1f);  // fade ind
        }
        else if (!shouldShow && okButton.activeSelf)
        {
            StartFade(0f);  // fade ud
        }
    }

    public void OnOkButtonPressed()
    {
        if (fishJourneyManager != null)
        {
            fishJourneyManager.ContinueJourney();
        }
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(targetAlpha));
    }

    IEnumerator FadeCanvasGroup(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (targetAlpha == 0f)
        {
            okButton.SetActive(false);
        }
    }
}

