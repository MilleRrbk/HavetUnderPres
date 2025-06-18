
using UnityEngine;
using System.Collections;

public class SpeechTrigger : MonoBehaviour
{
    public AudioSource audioSourceToPlay;
    public GameObject nextButtonUI;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        StartCoroutine(PlayAndShow());
    }

    private IEnumerator PlayAndShow()
    {
        Debug.Log("▶ Triggered!");

        if (audioSourceToPlay && audioSourceToPlay.clip != null)
        {
            audioSourceToPlay.Play();
            Debug.Log("▶ Playing: " + audioSourceToPlay.clip.name + ", duration: " + audioSourceToPlay.clip.length);

            yield return new WaitUntil(() => audioSourceToPlay.isPlaying);
            Debug.Log("🔊 Audio is now playing");

            yield return new WaitUntil(() => !audioSourceToPlay.isPlaying);
            Debug.Log("🛑 Audio finished");
        }
        else
        {
            Debug.LogWarning("⚠️ No AudioSource or clip set.");
        }

        if (nextButtonUI != null)
        {
            Debug.Log("✅ Showing next button!");
            nextButtonUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ nextButtonUI is not assigned!");
        }
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
