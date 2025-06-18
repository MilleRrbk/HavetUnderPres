using UnityEngine;

public class BillboardSubtitle : MonoBehaviour
{
    public Transform targetCamera; // typisk Main Camera (i XR Rig)

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // Placer canvas lidt foran kameraet
        transform.position = targetCamera.position + targetCamera.forward * 1.5f;

        // Peg canvas mod kameraet
        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.position);
    }
}