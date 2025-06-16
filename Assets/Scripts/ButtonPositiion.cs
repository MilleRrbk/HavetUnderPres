using UnityEngine;

public class UIFollowHead : MonoBehaviour
{
    public Transform headTransform;  // Dit VR headset (kamera) transform
    public Vector3 offset = new Vector3(0, -0.2f, 1f); // Justér efter behov
    public float smoothSpeed = 5f;

    void Update()
    {
        if (headTransform == null)
            return;

        Vector3 targetPos = headTransform.position + headTransform.forward * offset.z + headTransform.up * offset.y + headTransform.right * offset.x;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // Sørg for at Canvas vender mod hovedet
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - headTransform.position), Time.deltaTime * smoothSpeed);
    }
}