using UnityEngine;

public class LogoFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;
    [SerializeField] private Vector3 offset = new Vector3(0.6f, -0.5f, 2f); // højre-ned-foran

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main?.transform;
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // Beregn position ud fra kameraets lokale rum (så den følger hovedets retning)
        Vector3 worldOffset = targetCamera.TransformPoint(offset);
        transform.position = worldOffset;

        // Roter så den altid vender mod kameraet (uden at flade)
        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.position);
    }
}