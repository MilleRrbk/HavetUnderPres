using UnityEngine;

public class ProximitySelfController : MonoBehaviour
{
    public Transform xrOrigin;                // Reference til XR Origin
    public float activationDistance = 20f;    // Afstand i meter

    private MeshRenderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        // Automatisk find XR Origin hvis ikke sat manuelt
        if (xrOrigin == null)
        {
            GameObject xr = GameObject.Find("XR Origin");
            if (xr != null) xrOrigin = xr.transform;
        }

        // Find renderers og colliders på dette objekt og alle under-objekter
        renderers = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    void Update()
    {
        if (xrOrigin == null) return;

        float distance = Vector3.Distance(transform.position, xrOrigin.position);
        bool shouldBeVisible = distance < activationDistance;

        foreach (var r in renderers)
        {
            if (r != null && r.enabled != shouldBeVisible)
                r.enabled = shouldBeVisible;
        }

        foreach (var c in colliders)
        {
            if (c != null && c.enabled != shouldBeVisible)
                c.enabled = shouldBeVisible;
        }

        // Du kan aktivere/deaktivere hele objektet hvis du hellere vil det:
        // gameObject.SetActive(shouldBeVisible);
    }
}