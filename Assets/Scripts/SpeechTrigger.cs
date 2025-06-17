using UnityEngine;

public class SpeechTrigger : MonoBehaviour
{
    [Tooltip("AudioSource som skal afspilles, fx IntroTaleTorben")]
    public AudioSource audioSourceToPlay;

    [Tooltip("Reference til FishJourneyManager")]
    public FishJourneyManager fishJourney;

    [Tooltip("Next-knap der skal vises efter lyd")]
    public GameObject nextButtonUI;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        // Sørg for at XR Origin har tag "Player"
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        // Spil lyd
        if (audioSourceToPlay) audioSourceToPlay.Play();

        // Vis knap
        if (nextButtonUI) nextButtonUI.SetActive(true);

        // Fortæl fisken at den nu venter på input
        if (fishJourney) fishJourney.SetWaitingForInput(true);

        // Deaktiver triggeren så den ikke aktiveres igen
        gameObject.SetActive(false);
    }
}