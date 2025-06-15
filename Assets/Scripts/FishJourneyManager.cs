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
    public Transform playerTransform;   // XR Rig (not just camera)
    public AudioSource fishAudio;
    public AudioClip[] fishClips;

    [Header("Player Follow Settings")]
    public bool pullPlayerWithFish = true;
    public Vector3 followOffset = new Vector3(0, 1.5f, -3f);
    public float followSmoothTime = 0.15f;

    [Header("Constraints")]
    public float minHeight = 0.5f;

    private bool waitingForInput = false;
    private bool isMoving = true;
    private Vector3 playerVelocity = Vector3.zero;

    void Start()
    {
        StartCoroutine(MoveToNextPoint());
    }

    void Update()
    {
        if (!isMoving || currentIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        float step = moveSpeed * Time.deltaTime;

        // Clamp fish movement to not overshoot
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

        // Enforce min height
        Vector3 pos = transform.position;
        if (pos.y < minHeight)
        {
            pos.y = minHeight;
            transform.position = pos;
        }

        // Rotate fish smoothly (Y only)
        Vector3 flatDir = direction;
        flatDir.y = 0;

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDir) * Quaternion.Euler(0, 180f, 0);
            Quaternion correction = Quaternion.Euler(0, 90, 0);  // adjust if fish is rotated
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * correction, Time.deltaTime * rotationSpeed);
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    void LateUpdate()
    {
        // Smooth follow movement (no rotation override)
        if (pullPlayerWithFish && playerTransform != null)
        {
            Vector3 targetPosition = transform.TransformPoint(followOffset);
            targetPosition.y = playerTransform.position.y; // preserve XR headset height
            playerTransform.position = Vector3.SmoothDamp(playerTransform.position, targetPosition, ref playerVelocity, followSmoothTime);
        }
    }

    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f);

        // Fish looks at player (horizontal only)
        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0;
        Quaternion facePlayer = Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, -90f, 0);


        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, facePlayer, t);
            yield return null;
        }

        // Play sound
        if (fishClips.Length > currentIndex && fishAudio != null)
        {
            fishAudio.clip = fishClips[currentIndex];
            fishAudio.Play();
        }

        waitingForInput = true;
    }

    public void ContinueJourney()
    {
        if (!waitingForInput) return;

        waitingForInput = false;
        currentIndex++;

        if (currentIndex < waypoints.Length)
        {
            StartCoroutine(MoveToNextPoint());
        }
        else
        {
            Debug.Log("Fish has finished the journey.");
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
