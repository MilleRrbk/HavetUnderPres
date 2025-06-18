using UnityEngine;

public class FishSwimAround : MonoBehaviour
{
    [Header("Center to Swim Around")]
    public Transform target;

    [Header("Orbit Settings")]
    public float radiusX = 10f;
    public float radiusZ = 10f;
    public float angularSpeedDeg = 30f;     // grader pr. sekund
    public float angleOffsetDeg = 0f;       // startposition i kredsløb (grader)
    public float yOffset = 0f;              // højde over target
    public bool clockwise = true;

    private float angle;  // i radianer

    void Start()
    {
        float initialOffset = angleOffsetDeg * Mathf.Deg2Rad;
        angle = clockwise ? -initialOffset : initialOffset;
    }

    void Update()
    {
        if (target == null) return;

        float angularSpeedRad = angularSpeedDeg * Mathf.Deg2Rad;
        angle += (clockwise ? -1 : 1) * angularSpeedRad * Time.deltaTime;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radiusX,
            yOffset,
            Mathf.Sin(angle) * radiusZ
        );

        transform.position = target.position + offset;

        // Rotér fisken i kredsløbets retning
        Vector3 forwardDir = new Vector3(
            -Mathf.Sin(angle),
            0,
            Mathf.Cos(angle)
        );

        if (forwardDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forwardDir);
    }
}