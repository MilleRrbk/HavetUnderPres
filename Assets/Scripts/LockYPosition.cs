using UnityEngine;

[RequireComponent(typeof(Transform))]
public class LockYPosition : MonoBehaviour
{
    [Header("Fikseret højde (Y)")]
    [Tooltip("Den faste Y-højde, XR Origin skal holdes på")]
    public float fixedY = 3f;

    [Header("Debug")]
    public bool showDebugLogs = false;

    private void LateUpdate()
    {
        Vector3 currentPosition = transform.position;

        if (Mathf.Abs(currentPosition.y - fixedY) > 3.001f)
        {
            if (showDebugLogs)
                Debug.Log($"[LockYPosition] Justerer Y fra {currentPosition.y:F2} til {fixedY:F2}");

            currentPosition.y = fixedY;
            transform.position = currentPosition;
        }
    }
}

