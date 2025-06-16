using System.Collections;
using UnityEngine;

public class FishJourneyManager : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    private int currentIndex = 0;

    [Header("Fish Movement")]
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 2f;
    public float stopDistance = 0.5f;

    [Header("References")]
    public AudioSource fishAudio;
    public AudioClip[] fishClips;
    public GameObject nextButtonUI;  // 👈 UI Button that appears when fish stops

    [Header("Constraints")]
    public float minHeight = 0.5f;

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
        if (!isMoving || currentIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        float step = moveSpeed * Time.deltaTime;

        // Move toward the waypoint
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

        // Keep fish above min height
        Vector3 pos = transform.position;
        if (pos.y < minHeight)
        {
            pos.y = minHeight;
            transform.position = pos;
        }

        // Rotate only on Y-axis
        Vector3 flatDir = direction;
        flatDir.y = 0;

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0); // Adjust fish facing direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * correction, Time.deltaTime * rotationSpeed);
        }

        // Arrived at waypoint
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f);

        // Play sound for this waypoint
        if (fishClips.Length > currentIndex && fishAudio != null)
        {
            fishAudio.clip = fishClips[currentIndex];
            fishAudio.Play();
        }

        // Show the "Next" button
        if (nextButtonUI != null)
            nextButtonUI.SetActive(true);

        waitingForInput = true;
    }

    public void ContinueJourney()
    {
        if (!waitingForInput) return;

        waitingForInput = false;

        // Hide the "Next" button
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
