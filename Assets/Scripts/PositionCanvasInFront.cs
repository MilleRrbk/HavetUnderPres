using UnityEngine;

public class PositionCanvasInFront : MonoBehaviour
{
    public float distanceFromCamera = 2f;

    void Start()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 forward = cam.transform.forward;
            Vector3 position = cam.transform.position + forward * distanceFromCamera;
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}   