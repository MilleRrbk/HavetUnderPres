using System.Collections;
using UnityEngine;

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
    public AudioSource  introSpeechSource;   // Spilles KUN ved waypoint 0
    public AudioSource[] fishSources;        // Ét AudioSource pr. EFTERFØLGENDE waypoint
    public GameObject   nextButtonUI;        // Knappen der vises når fisken stopper

    // ─── Constraints ─────────────────────────────────────────────
    [Header("Constraints")]
    public float minHeight = 0.5f;

    // ─── Intern tilstand ─────────────────────────────────────────
    private bool waitingForInput = false;
    private bool isMoving        = true;

    // ─────────────────────────────────────────────────────────────
    //  INITIALISERING
    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (waypoints.Length == 0) return;

        // Hvis fisken starter meget tæt på waypoint 0
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
    //  OPDATERING
    // ─────────────────────────────────────────────────────────────
    void Update()
    {
        if (!isMoving || currentIndex >= waypoints.Length) return;

        Transform target   = waypoints[currentIndex];
        Vector3   direction = target.position - transform.position;
        float     step      = moveSpeed * Time.deltaTime;

        // Bevæg fisken
        if (direction.magnitude <= step)
            transform.position = target.position;
        else
            transform.position += new Vector3(direction.x, 0, direction.z).normalized * step;

        // Højde-begrænsning
        Vector3 pos = transform.position;
        if (pos.y < minHeight) { pos.y = minHeight; transform.position = pos; }

        // Rotation (kun Y-akse)
        Vector3 faceDir = new Vector3(direction.x, 0, direction.z);
        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot  = Quaternion.LookRotation(faceDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0); // fiskens model retning
            transform.rotation    = Quaternion.Slerp(transform.rotation,
                                                     targetRot * correction,
                                                     Time.deltaTime * rotationSpeed);
        }

        // Nået waypoint?
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  HÅNDTER ANKOMST TIL WAYPOINT
    // ─────────────────────────────────────────────────────────────
// ─────────────────────────────────────────────────────────────
//  HÅNDTER ANKOMST TIL WAYPOINT
// ─────────────────────────────────────────────────────────────
IEnumerator HandleArrival()
{
    yield return new WaitForSeconds(0.5f);           // lille pause

    // ── WAYPOINT 0: INTROTALEN ───────────────────────────────
    if (currentIndex == 0 && introSpeechSource != null)
    {
        introSpeechSource.Play();
        // ➜ vent helt til lyden er færdig
        yield return new WaitUntil(() => !introSpeechSource.isPlaying);
    }
    // ── ØVRIGE WAYPOINTS ────────────────────────────────────
    else
    {
        // evt. kort pause før talen starter
        yield return new WaitForSeconds(0.5f);

        // start tale
        AudioSource src = PlaySpeechForCurrentIndex();

        // ➜ vent til klippet stopper
        if (src != null)                                      
            yield return new WaitUntil(() => !src.isPlaying); 
    }

    // ── NU vises knappen ─────────────────────────────────────
    if (nextButtonUI != null)
        nextButtonUI.SetActive(true);

    waitingForInput = true;
}


    // ─────────────────────────────────────────────────────────────
    //  AFSPLIL KORREKT TALE PR. WAYPOINT (EFTER INTRO)
    // ─────────────────────────────────────────────────────────────
 // Ændr returtypen så vi kan få referencen
private AudioSource PlaySpeechForCurrentIndex()
{
    foreach (AudioSource s in fishSources)            // stop alle andre
        if (s) s.Stop();

    int idx = currentIndex - 1;                       // fishSources starter ved wp 1
    if (idx >= 0 && idx < fishSources.Length && fishSources[idx] != null)
    {
        fishSources[idx].Play();
        return fishSources[idx];                      // giver ref. tilbage
    }
    return null;
}


    // ─────────────────────────────────────────────────────────────
    //  UI-KNAP KALDER DENNE
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
            Debug.Log("Fisken er færdig med rejsen!");
    }

    // ─────────────────────────────────────────────────────────────
    //  FORTSÆT TIL NÆSTE WAYPOINT
    // ─────────────────────────────────────────────────────────────
    IEnumerator MoveToNextPoint()
    {
        yield return new WaitForSeconds(0.5f); // lille forsinkelse
        isMoving = true;
    }

    // ─────────────────────────────────────────────────────────────
    //  EKSTERN KONTROL FRA TRIGGERS
    // ─────────────────────────────────────────────────────────────
    public void SetWaitingForInput(bool value) => waitingForInput = value;

    public bool IsWaitingForInput() => waitingForInput;
}
