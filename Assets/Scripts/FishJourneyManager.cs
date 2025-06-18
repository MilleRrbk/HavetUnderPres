using UnityEngine;
using System.Collections;

public class FishJourneyManager : MonoBehaviour
{
    // ─── Waypoints ───────────────────────────────────────────────
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int currentIndex = 0;

    // ─── Movement ────────────────────────────────────────────────
    [Header("Fish Movement")]
    public float moveSpeed     = 1.5f;
    public float rotationSpeed = 2f;
    public float stopDistance  = 0.5f;

    // ─── Audio & UI ──────────────────────────────────────────────
    [Header("Audio & UI")]
    public AudioSource  introSpeechSource;
    public AudioSource[] fishSources;
    public GameObject   nextButtonUI;

    // ─── Constraints ─────────────────────────────────────────────
    [Header("Constraints")]
    public float minHeight = 0.5f;

    // ─── Internal State ──────────────────────────────────────────
    private bool waitingForInput = false;
    private bool isMoving        = true;

    void Start()
    {
        if (nextButtonUI != null)
            nextButtonUI.SetActive(false);

        if (waypoints.Length == 0) return;

        if (Vector3.Distance(transform.position, waypoints[0].position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
        else
        {
            StartCoroutine(MoveToNextPoint());
        }
    }

    void Update()
    {
        if (!isMoving || currentIndex >= waypoints.Length) return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        float step = moveSpeed * Time.deltaTime;

        if (direction.magnitude <= step)
            transform.position = target.position;
        else
            transform.position += new Vector3(direction.x, 0, direction.z).normalized * step;

        Vector3 pos = transform.position;
        if (pos.y < minHeight) { pos.y = minHeight; transform.position = pos; }

        Vector3 faceDir = new Vector3(direction.x, 0, direction.z);
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot  = Quaternion.LookRotation(faceDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot * correction, Time.deltaTime * rotationSpeed);
        }

        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f);

        if (currentIndex == 0 && introSpeechSource != null)
        {
            introSpeechSource.Play();
            yield return new WaitUntil(() => !introSpeechSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            AudioSource src = PlaySpeechForCurrentIndex();
            if (src != null)
                yield return new WaitUntil(() => !src.isPlaying);
        }

        bool isLastWaypoint = currentIndex >= waypoints.Length - 1;
     	waitingForInput = !isLastWaypoint;

    }

    private AudioSource PlaySpeechForCurrentIndex()
    {
        foreach (AudioSource s in fishSources)
            if (s) s.Stop();

        int idx = currentIndex - 1;
        if (idx >= 0 && idx < fishSources.Length && fishSources[idx] != null)
        {
            fishSources[idx].Play();
            return fishSources[idx];
        }
        return null;
    }

    public void ContinueJourney()
    {
        if (!waitingForInput) return;

        waitingForInput = false;
        if (nextButtonUI != null)
            nextButtonUI.SetActive(false);

        currentIndex++;

        if (currentIndex < waypoints.Length)
            StartCoroutine(MoveToNextPoint());
        else
            Debug.Log("Fisken er færdig med rejsen!");
    }

    IEnumerator MoveToNextPoint()
    {
        yield return new WaitForSeconds(0.5f);
        isMoving = true;
    }

    public void TriggerSpeech()
    {
        if (!isMoving && !IsLastWaypoint())
        {
            StartCoroutine(HandleArrival());
        }
    }

    public void SetWaitingForInput(bool value) => waitingForInput = value;
    public bool IsWaitingForInput() => waitingForInput;
    public bool IsLastWaypoint() => currentIndex >= waypoints.Length - 1;
}
