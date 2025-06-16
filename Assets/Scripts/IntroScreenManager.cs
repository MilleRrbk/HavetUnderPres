using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/// <summary>
/// Viser en sløret introskærm oven på XR-kameraet, låser tiden,
/// og fjerner alt igen, når brugeren klikker "Start".
/// </summary>
public class IntroScreenManager : MonoBehaviour
{
    [Header("UI-referencer (drag-&-drop)")]
    [SerializeField] private Canvas introCanvas;    // Hele Canvas-enhed
    [SerializeField] private GameObject blurPanel;  // Panel med UniversalBlurUI-materiale
    [SerializeField] private Button startButton;    // "Start"-knappen

    [Header("Valgfri post-processing")]
    [SerializeField] private Volume blurVolume;     // URP Global Volume med Depth Of Field (kan være null)

    [Header("Adfærd")]
    [SerializeField] private bool pauseTime = true; // Stop Time.timeScale mens intro er aktiv

    private void Awake()
    {
        // Vis UI + blur ved spilstart
        introCanvas.enabled = true;
        if (blurPanel   != null) blurPanel.SetActive(true);
        if (blurVolume  != null) blurVolume.enabled = true;

        if (pauseTime) Time.timeScale = 0f;          // fryser alt gameplay

        // Knyt klik-håndtering
        if (startButton != null)
            startButton.onClick.AddListener(StartExperience);
    }

    /// <summary>
    /// Kaldes, når brugeren trykker på "Start".
    /// Skjuler introen og genoptager spillet.
    /// </summary>
    public void StartExperience()
    {
        if (pauseTime) Time.timeScale = 1f;

        // Fjern UI og blur
        introCanvas.enabled = false;
        if (blurPanel  != null) blurPanel.SetActive(false);
        if (blurVolume != null) blurVolume.enabled = false;

        // Afmeld lytter og deaktivér dette script (ikke strengt nødvendigt)
        if (startButton != null)
            startButton.onClick.RemoveListener(StartExperience);

        enabled = false;
    }
}