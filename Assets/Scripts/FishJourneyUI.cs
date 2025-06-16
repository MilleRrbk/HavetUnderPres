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

    private Transform canvasTransform;
    private Camera mainCamera;

    void Awake()
    {
        canvasTransform = transform;
        mainCamera = Camera.main; // Hovedkameraet (VR headset)

        if (okButton != null)
        {
            canvasGroup = okButton.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = okButton.AddComponent<CanvasGroup>();
            }

            // Start skjult og ikke interaktiv
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
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
            StartFade(1f);  // Fade in
        }
        else if (!shouldShow && okButton.activeSelf)
        {
            StartFade(0f);  // Fade out
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null)
            return;

        // Placer canvas 1.5 meter foran kameraet, og drej det mod kameraet
        canvasTransform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.5f;

        // Sørg for canvas "kigger mod" kameraet (så det altid vender rigtigt)
        canvasTransform.rotation = Quaternion.LookRotation(canvasTransform.position - mainCamera.transform.position);
    }

   public void OnOkButtonPressed()
{
    Debug.Log("Knappen blev trykket!");  // <--- Her ser vi om klik kommer igennem

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

        if (targetAlpha == 1f)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            okButton.SetActive(false);
        }
    }
}
