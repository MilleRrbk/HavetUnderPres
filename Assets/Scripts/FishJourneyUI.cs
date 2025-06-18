using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FishJourneyUI : MonoBehaviour
{
    [Header("References")]
    public FishJourneyManager fishJourneyManager;
    public GameObject okButton;

    [Header("UI Behaviour")]
    public float fadeDuration = 0.5f;

    CanvasGroup canvasGroup;
    Coroutine fadeRoutine;
    Camera mainCam;
    Transform t;

    void Awake()
    {
        mainCam = Camera.main;
        t = transform;

        if (okButton == null)
        {
            Debug.LogError("FishJourneyUI: OkButton er ikke sat i Inspector!");
            enabled = false;
            return;
        }

        canvasGroup = okButton.GetComponent<CanvasGroup>() ?? okButton.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        okButton.SetActive(false);
    }

    void Update()
    {
        if (fishJourneyManager == null) return;

        bool shouldShow = fishJourneyManager.IsWaitingForInput();

        if (shouldShow && !okButton.activeSelf)
        {
            ShowButton();
        }
        else if (!shouldShow && okButton.activeSelf)
        {
            HideButton();
        }
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        t.position = mainCam.transform.position + mainCam.transform.forward * 1.5f;
        t.rotation = Quaternion.LookRotation(t.position - mainCam.transform.position);
    }

    public void OnOkButtonPressed()
    {
        Debug.Log("FishJourneyUI: OK‑knap trykket");
        if (fishJourneyManager != null)
        {
            fishJourneyManager.ContinueJourney();
        }
        else
        {
            Debug.LogWarning("FishJourneyUI: FishJourneyManager mangler!");
        }
    }

    void ShowButton()
    {
        okButton.SetActive(true);
        StartFade(1f, true);
    }

    void HideButton()
    {
        StartFade(0f, false);
    }

    void StartFade(float targetAlpha, bool enableRaycast)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(targetAlpha, enableRaycast));
    }

    IEnumerator FadeCanvas(float targetAlpha, bool enableRaycast)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.interactable = enableRaycast;
        canvasGroup.blocksRaycasts = enableRaycast;

        if (targetAlpha == 0f)
            okButton.SetActive(false);
    }
}
