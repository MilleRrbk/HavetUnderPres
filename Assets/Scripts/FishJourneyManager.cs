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
    public Transform playerTransform;   // XR Rig eller spillerens root transform
    public AudioSource fishAudio;
    public AudioClip[] fishClips;

    [Header("Player Follow Settings")]
    public Vector3 followOffset = new Vector3(0, 1.5f, -3f);
    public float followSmoothTime = 0.15f;

    [Header("Constraints")]
    public float minHeight = 0.5f;

    private bool waitingForInput = false;
    private bool isMoving = true;

    private Vector3 playerVelocity = Vector3.zero;

    void Start()
    {
        // Vi parenter ikke spilleren mere!
        // Start rejsen:
        StartCoroutine(MoveToNextPoint());
    }

    void Update()
    {
        if (!isMoving || currentIndex >= waypoints.Length)
            return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = target.position - transform.position;
        float step = moveSpeed * Time.deltaTime;

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

        // Minimum højde
        Vector3 pos = transform.position;
        if (pos.y < minHeight)
        {
            pos.y = minHeight;
            transform.position = pos;
        }

        // Rotation kun på Y-aksen
        Vector3 flatDir = direction;
        flatDir.y = 0;

        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDir);
            Quaternion correction = Quaternion.Euler(0, 270, 0); // 90 + 180 = 270 degrees
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * correction, Time.deltaTime * rotationSpeed);
        }

        // Check om vi er ved waypoint
        if (Vector3.Distance(transform.position, target.position) < stopDistance)
        {
            isMoving = false;
            StartCoroutine(HandleArrival());
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Smooth follow spiller bag fisken i en offset-position
        Vector3 targetPosition = transform.TransformPoint(followOffset);
        // Bevar spillerens y-position (f.eks. headsetets højde)
        targetPosition.y = playerTransform.position.y;

        playerTransform.position = Vector3.SmoothDamp(playerTransform.position, targetPosition, ref playerVelocity, followSmoothTime);
    }

    IEnumerator HandleArrival()
    {
        yield return new WaitForSeconds(0.5f);

        // Fisken kigger mod spilleren horisontalt
        Vector3 lookDir = playerTransform.position - transform.position;
        lookDir.y = 0;
        Quaternion facePlayer = Quaternion.LookRotation(lookDir);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, facePlayer, t);
            yield return null;
        }

        // Spil lyd
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
