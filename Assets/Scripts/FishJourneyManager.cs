using System.Collections;
using UnityEngine;

public class FishJourneyManager : MonoBehaviour
{
    // ──────────── Waypoints ────────────
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int currentIndex = 0;

    // ──────────── Movement ────────────
    [Header("Fish Movement")]
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 2f;
    public float stopDistance = 0.5f;

    // ──────────── Audio & UI ────────────
    [Header("References")]
    public AudioSource[] fishSources;   // Ét AudioSource-objekt pr. waypoint
    public GameObject nextButtonUI;     // Knappen der vises når fisken stopper

    // ──────────── Constraints ────────────
    [Header("Constraints")]
    public float minHeight = 0.5f;

    // ──────────── Intern tilstand ────────────
    private bool waitingForInput = false;
    private bool isMoving = true;

    void Start()
    {
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

        // ─── Bevægelse ───
        if (direction.magnitude <= step)
            transform.position = target.position;
        else
        {
            Vector3 flatDir = direction; flatDir.y = 0;
            transform.position += flatDir.normalized * step;
        }

        // Højde-begrænsning
        Vector3 pos = transform.position;
        if (pos.y < minHeight) { pos.y = minHeight; transform.position = pos; }

        // Rotation kun på Y
        Vector3 faceDir = direction; faceDir.y = 0;
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot * correction, Time.deltaTime * rotationSpeed);
        }

        // Ankommet til waypoint
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());   // fisk stopper; lyd/knap styres her ELLER via trigger
        }
    }

    // ─── Når fisken rammer waypointet ───
    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f);   // lille pause
        // 7 sekunders ro før taleklippet (kan fjernes hvis du bruger trigger i stedet)
        yield return new WaitForSeconds(7f);

        PlaySpeechForCurrentIndex();

        // Vis knap
        if (nextButtonUI) nextButtonUI.SetActive(true);

        waitingForInput = true;
    }

    // Spiller lyd fra det AudioSource-objekt der svarer til currentIndex
    private void PlaySpeechForCurrentIndex()
    {
        // Stop alle andre
        foreach (AudioSource src in fishSources) if (src) src.Stop();

        if (fishSources.Length > currentIndex && fishSources[currentIndex])
            fishSources[currentIndex].Play();
    }

    public void ContinueJourney()   // kaldes af knappen
    {
        if (!waitingForInput) return;

        waitingForInput = false;
        if (nextButtonUI) nextButtonUI.SetActive(false);

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

    // ─── Bruges af SpeechTrigger ───
    public void SetWaitingForInput(bool value)
    {
        waitingForInput = value;
    }

    public bool IsWaitingForInput() => waitingForInput;
}
