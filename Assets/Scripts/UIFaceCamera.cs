using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    [SerializeField] private Transform targetCamera; // Træk Main Camera ind her
    [SerializeField] private float distance = 2f;     // Hvor langt foran kameraet canvas skal placeres
    [SerializeField] private bool faceCamera = true;  // Skal UI også rotere mod kamera?

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        // Positioner canvas foran kamera
        Vector3 forward = targetCamera.forward;
        Vector3 position = targetCamera.position + forward * distance;
        transform.position = position;

        // Rotér canvas så det altid kigger mod kamera
        if (faceCamera)
        {
            transform.rotation = Quaternion.LookRotation(forward);
        }
    }
}