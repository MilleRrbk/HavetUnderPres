using UnityEngine;

public class ProximityManager : MonoBehaviour
{
    public Transform xrOrigin;
    public float activationDistance = 20f;
    public string targetTag = "LODObject"; // Hvilke objekter skal proximity-styres

    private GameObject[] proximityObjects;

    void Start()
    {
        if (xrOrigin == null)
        {
            GameObject found = GameObject.Find("XR Origin");
            if (found != null) xrOrigin = found.transform;
        }

        // Find alle objekter med det rigtige tag
        proximityObjects = GameObject.FindGameObjectsWithTag(targetTag);

        Debug.Log($"🔍 Found {proximityObjects.Length} objects with tag '{targetTag}'");
        foreach (var obj in proximityObjects)
        {
            Debug.Log($"🧩 Found: {obj.name}");
        }
    }

    void Update()
    {
        if (xrOrigin == null || proximityObjects == null) return;

        foreach (GameObject obj in proximityObjects)
        {
            if (obj == null) continue;

            float dist = Vector3.Distance(obj.transform.position, xrOrigin.position);
            bool shouldBeActive = dist < activationDistance;

            // 🔎 Log aktiv-status og parent-status
            string parentName = obj.transform.parent ? obj.transform.parent.name : "(no parent)";
            Debug.Log($"{obj.name} (parent: {parentName}) → Active: {obj.activeSelf}, Distance: {dist}");

            // 🛠️ Hvis objektet har parent med mesh eller visuelle ting, prøv at deaktivere hele parent
            GameObject targetToSet = obj;

            if (obj.transform.parent != null &&
                obj.transform.parent.GetComponent<MeshRenderer>() != null)
            {
                targetToSet = obj.transform.parent.gameObject;
            }

            if (targetToSet.activeSelf != shouldBeActive)
            {
                Debug.Log($"↔ Changing {targetToSet.name} active = {shouldBeActive}");
                var renderers = targetToSet.GetComponentsInChildren<MeshRenderer>();
                foreach (var r in renderers)
                    r.enabled = shouldBeActive;            }
        }
    }
}
