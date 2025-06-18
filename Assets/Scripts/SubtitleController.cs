using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleController : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    public float distance = 1.5f;     // 1.5 m foran kamera
    public float defaultDuration = 2f;

    Camera cam;
    Transform t;

    void Awake()
    {
        cam = Camera.main;
        t = transform;
        if (subtitleText != null) subtitleText.text = "";
    }

    void LateUpdate()
    {
        if (cam == null) return;
        t.position  = cam.transform.position + cam.transform.forward * distance;
        t.rotation  = Quaternion.LookRotation(t.position - cam.transform.position);
    }

    public void ShowSequence(string raw, float totalTime)
    {
        StopAllCoroutines();
        StartCoroutine(Sequence(raw, totalTime));
    }

    IEnumerator Sequence(string raw, float totalTime)
    {
        if (subtitleText == null || string.IsNullOrEmpty(raw)) yield break;

        string[] parts = raw.Split('|');
        float seg = (totalTime > 0f) ? totalTime / parts.Length : defaultDuration;

        foreach (string p in parts)
        {
            subtitleText.text = p.Trim();
            yield return new WaitForSeconds(seg);
        }
        subtitleText.text = "";
    }
}