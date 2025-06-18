using UnityEngine;
using System.Collections;


public class SpeechTrigger : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject nextButton;
    public SubtitleController subtitleController;

    [TextArea(3,10)] public string[] subtitleLines;   // Sætninger
    public float[] durations;                         // Hvor længe hver skal vises

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (subtitleController != null)
        {
            subtitleController.Hide(); // ryd evt. tidligere tekst
            subtitleController.ShowSequence(subtitleLines, durations);
        }

        if (audioSource != null)
            audioSource.Play();

        yield return new WaitUntil(() => !audioSource.isPlaying);

        if (nextButton != null)
            nextButton.SetActive(true);

        gameObject.SetActive(false);
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
