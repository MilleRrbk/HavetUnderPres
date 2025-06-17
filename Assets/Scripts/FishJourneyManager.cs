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
    public AudioSource[] fishSources;        // 🎧 Træk dine AudioSource-objekter herind (fx IntroTaleTorben osv.)
    public GameObject nextButtonUI;

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

        // Bevægelse
        if (direction.magnitude <= step)
        {
            transform.position = target.position;
        }
        else
        {
            Vector3 flatDirection = direction;
            flatDirection.y = 0;
            flatDirection.Normalize();

            transform.position += flatDirection * step;
        }

        // Højde-begrænsning
        Vector3 pos = transform.position;
        if (pos.y < minHeight)
        {
            pos.y = minHeight;
            transform.position = pos;
        }

        // Rotation
        Vector3 flatDir = direction;
        flatDir.y = 0;

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * correction, Time.deltaTime * rotationSpeed);
        }

        // Ankommet?
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f); // Pause efter ankomst
        yield return new WaitForSeconds(1f);   // Vent 7 sekunder før tale

        // Stop alle lyde (for en sikkerheds skyld)
        foreach (AudioSource src in fishSources)
        {
            if (src != null) src.Stop();
        }

        // Afspil den rigtige lyd
        if (fishSources.Length > currentIndex && fishSources[currentIndex] != null)
        {
            fishSources[currentIndex].Play();
        }

        // Vis knap
        if (nextButtonUI != null)
            nextButtonUI.SetActive(true);

        waitingForInput = true;
    }

    public void ContinueJourney()
    {
        if (!waitingForInput) return;

        waitingForInput = false;

        if (nextButtonUI != null)
            nextButtonUI.SetActive(false);

        currentIndex++;

        if (currentIndex < waypoints.Length)
        {
            StartCoroutine(MoveToNextPoint());
        }
        else
        {
            Debug.Log("Fisken er færdig med rejsen!");
        }
    }

    IEnumerator MoveToNextPoint()
    {
        yield return new WaitForSeconds(0.5f);
        isMoving = true;
    }

    public bool IsWaitingForInput()
    {
        return waitingForInput;
    }
}
