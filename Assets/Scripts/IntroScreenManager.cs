using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Viser en sløret introskærm og låser spillet, indtil brugeren klikker "Start".
/// </summary>
public class IntroScreenManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas introCanvas;          // Hele Canvas'en
    [SerializeField] private Volume volume;               // Global Volume med Depth of Field
    [SerializeField] private GameObject blurTarget;       // Panelet med UI Blur (kan også være null)
    
    [Header("Optional")]
    [SerializeField] private bool lockTime = true;        // Stop tid mens intro vises

    private void Awake()
    {
        // Sørg for at være aktiv fra start
        introCanvas.enabled = true;
        if (volume != null) volume.enabled = true;
        if (blurTarget != null) blurTarget.SetActive(true);

        if (lockTime) Time.timeScale = 0f; // fryser alt gameplay
    }

    /// <summary>
    /// Kaldes af Start-knappens OnClick().
    /// </summary>
    public void StartExperience()
    {
        // Fjern blur + UI
        introCanvas.enabled = false;
        if (volume != null) volume.enabled = false;
        if (blurTarget != null) blurTarget.SetActive(false);

        if (lockTime) Time.timeScale = 1f; // genoptag gameplay
    }
} 