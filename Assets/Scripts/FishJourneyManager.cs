using System.Collections;
using UnityEngine;

public class FishJourneyManager : MonoBehaviour
{
    // ─── waypoints ───────────────────────────────────────────────
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int currentIndex = 0;

    // ─── movement ────────────────────────────────────────────────
    [Header("Fish Movement")]
    public float moveSpeed     = 1.5f;
    public float rotationSpeed = 2f;
    public float stopDistance  = 0.5f;

    // ─── audio & ui ──────────────────────────────────────────────
    [Header("Audio & UI")]
    public AudioSource  introSpeechSource;   // spiller kun ved waypoint 0
    public AudioSource[] fishSources;        // ét audiosource pr. efterfølgende waypoint
    public GameObject   nextButtonUI;        // knappen der vises når fisken stopper

    // ─── constraints ─────────────────────────────────────────────
    [Header("Constraints")]
    public float minHeight = 0.5f;

    // ─── intern tilstand ─────────────────────────────────────────
    private bool waitingForInput = false;
    private bool isMoving        = true;

    // ─────────────────────────────────────────────────────────────
    //  initialisering
    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (waypoints.Length == 0) return;

        // hvis fisken starter tæt på waypoint 0
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

    // ─────────────────────────────────────────────────────────────
    //  opdatering
    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (!isMoving || currentIndex >= waypoints.Length) return;

        Transform target   = waypoints[currentIndex];
        Vector3   direction = target.position - transform.position;
        float     step      = moveSpeed * Time.deltaTime;

        // bevæg fisken
        if (direction.magnitude <= step)
            transform.position = target.position;
        else
            transform.position += new Vector3(direction.x, 0, direction.z).normalized * step;

        // højde-begrænsning
        Vector3 pos = transform.position;
        if (pos.y < minHeight) { pos.y = minHeight; transform.position = pos; }

        // rotation (kun y-akse)
        Vector3 faceDir = new Vector3(direction.x, 0, direction.z);
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot  = Quaternion.LookRotation(faceDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0); // fiskens model-retning
            transform.rotation    = Quaternion.Slerp(transform.rotation,
                                                     targetRot * correction,
                                                     Time.deltaTime * rotationSpeed);
        }

        // nået waypoint?
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  håndter ankomst til waypoint
    // ─────────────────────────────────────────────────────────────
    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f); // lille pause

        // waypoint 0: introtale
        if (currentIndex == 0 && introSpeechSource != null)
        {
            introSpeechSource.Play();
            yield return new WaitUntil(() => !introSpeechSource.isPlaying);
        }
        // øvrige waypoints
        else
        {
            yield return new WaitForSeconds(0.5f); // kort pause
            AudioSource src = PlaySpeechForCurrentIndex();
            if (src != null)
                yield return new WaitUntil(() => !src.isPlaying);
        }

        // kun vis knappen hvis der er flere waypoints tilbage
        bool isLastWaypoint = currentIndex >= waypoints.Length - 1;
        if (!isLastWaypoint && nextButtonUI != null)
            nextButtonUI.SetActive(true);

        waitingForInput = true;
    }

    // ─────────────────────────────────────────────────────────────
    //  afspil korrekt tale pr. waypoint
    // ─────────────────────────────────────────────────────────────
    private AudioSource PlaySpeechForCurrentIndex()
    {
        foreach (AudioSource s in fishSources)
            if (s) s.Stop();

        int idx = currentIndex - 1; // fishSources starter ved wp 1
        if (idx >= 0 && idx < fishSources.Length && fishSources[idx] != null)
        {
            fishSources[idx].Play();
            return fishSources[idx];
        }
        return null;
    }

    // ─────────────────────────────────────────────────────────────
    //  ui-knap kalder denne
    // ─────────────────────────────────────────────────────────────
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
            Debug.Log("fisken er færdig med rejsen!");
    }

    // ─────────────────────────────────────────────────────────────
    //  fortsæt til næste waypoint
    // ─────────────────────────────────────────────────────────────
    IEnumerator MoveToNextPoint()
    {
        yield return new WaitForSeconds(0.5f); // lille forsinkelse
        isMoving = true;
    }

    // ─────────────────────────────────────────────────────────────
    //  ekstern kontrol fra triggers
    // ─────────────────────────────────────────────────────────────
    public void SetWaitingForInput(bool value) => waitingForInput = value;
    public bool IsWaitingForInput()           => waitingForInput;
}
