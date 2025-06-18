using System.Collections;
using System.Collections;
using UnityEngine;
using TMPro;

public class SubtitleController : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;

    private Coroutine currentRoutine;

    public void ShowSequence(string[] lines, float[] durations)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            subtitleText.text = ""; // ryd skærmen straks
        }

        currentRoutine = StartCoroutine(SubtitleRoutine(lines, durations));
    }

    public void Hide()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        subtitleText.text = "";
    }

    IEnumerator SubtitleRoutine(string[] lines, float[] durations)
    {
        subtitleText.text = "";

        for (int i = 0; i < lines.Length; i++)
        {
            subtitleText.text = lines[i];
            yield return new WaitForSeconds(durations[i]);
        }

        subtitleText.text = "";
        currentRoutine = null;
    }
}
