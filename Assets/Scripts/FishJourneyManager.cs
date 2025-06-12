using System.Collections;
using UnityEngine;

public class FishJourneyManager : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints; // Træk waypoints ind i editoren
    private int currentIndex = 0;

    [Header("Fish Movement")]
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 2f;
    public float stopDistance = 0.5f;

    [Header("References")]
    public Transform playerTransform;     // Træk XR Rig eller kamera herind
    public AudioSource fishAudio;         // AudioSource med tale/lyd
    public AudioClip[] fishClips;         // Én clip pr stop

    private bool waitingForInput = false;
    private bool isMoving = true;

    void Start()
    {
        AttachPlayer();
        StartCoroutine(MoveToNextPoint());
    }

    void Update()
    {
        if (!isMoving || currentIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    IEnumerator HandleArrival()
    {
        DetachPlayer(); // Så spilleren står frit under stop

        // Kig mod spilleren
        yield return new WaitForSeconds(0.5f);
        Vector3 lookDir = (playerTransform.position - transform.position).normalized;
        lookDir.y = 0; // Begræns rotation til horisontal
        Quaternion facePlayer = Quaternion.LookRotation(lookDir);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, facePlayer, t);
            yield return null;
        }

        // Spil tale/lyd
        if (fishClips.Length > currentIndex && fishAudio != null)
        {
            fishAudio.clip = fishClips[currentIndex];
            fishAudio.Play();
        }

        // Vent på brugerens bekræftelse
        waitingForInput = true;
    }

    public void ContinueJourney()
    {
        if (!waitingForInput) return;

        waitingForInput = false;
        currentIndex++;

        if (currentIndex < waypoints.Length)
        {
            AttachPlayer();
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

    private void AttachPlayer()
    {
        playerTransform.SetParent(this.transform);
    }

    private void DetachPlayer()
    {
        playerTransform.SetParent(null);
    }
}
