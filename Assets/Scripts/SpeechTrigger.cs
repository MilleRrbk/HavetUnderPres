using UnityEngine;

public class SpeechTrigger : MonoBehaviour
{
    public FishJourneyManager fishJourney;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (fishJourney != null)
        {
            fishJourney.TriggerSpeech();
        }

        gameObject.SetActive(false); // valgfrit: deaktiver trigger
    }
}


/*
using UnityEngine;
using System.Collections;

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
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        // Start coroutine i stedet for direkte aktivering
        StartCoroutine(PlayAndShow());
        gameObject.SetActive(false);          // deaktiver triggeren
    }

    
    private IEnumerator PlayAndShow()
    {
        if (audioSourceToPlay && audioSourceToPlay.clip != null)
        {
            audioSourceToPlay.Play();

            // Wait until playback actually begins
            yield return new WaitUntil(() => audioSourceToPlay.isPlaying);

            // Then wait until playback ends
            yield return new WaitUntil(() => !audioSourceToPlay.isPlaying);
        }

        if (nextButtonUI) nextButtonUI.SetActive(true);
        if (fishJourney) fishJourney.SetWaitingForInput(true);
    }




    
}
*/
