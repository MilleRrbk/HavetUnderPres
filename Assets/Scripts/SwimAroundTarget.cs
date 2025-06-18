using UnityEngine;

public class SwimAroundTarget : MonoBehaviour
{
    [Header("Target to swim around")]
    public Transform targetBoat;          // Båd- eller midtpunkt

    [Header("Path settings")]
    public float radiusX = 8f;            // “bredde” på kredsløbet
    public float radiusZ = 6f;            // “længde” på kredsløbet
    public float angularSpeedDeg = 30f;   // grader pr. sekund
    public float startAngleDeg = 0f;      // hvor i cirklen stimen starter

    [Tooltip("Hvis du vil specificere egne punkter i stedet for en cirkel, læg dem her (X = offset i X, Y = offset i Z). Efterlades tomt => almindelig cirkel/ellipse.")]
    public Vector2[] customPath;          // valgfrit – lader dig tegne din egen bane

    private float angle;                  // løbende vinkel
    private int pathIndex = 0;            // til customPath

    void Start()
    {
        angle = startAngleDeg * Mathf.Deg2Rad;

        if (targetBoat == null)
            Debug.LogWarning("❗  targetBoat mangler – sæt bådens Transform i Inspector.");
    }

    void Update()
    {
        if (targetBoat == null) return;

        // ───── Hent center (bådens position) ─────
        Vector3 center = targetBoat.position;

        // ───── Udregn næste position ─────
        Vector3 nextPos;

        if (customPath != null && customPath.Length > 0)
        {
            // Gå igennem punkter ét efter ét
            Vector2 offset = customPath[pathIndex];
            nextPos = center + new Vector3(offset.x, 0, offset.y);

            // Næste punkt
            pathIndex = (pathIndex + 1) % customPath.Length;
        }
        else
        {
            // Elliptisk bane baseret på radiusX / radiusZ
            angle += angularSpeedDeg * Mathf.Deg2Rad * Time.deltaTime;

            float x = Mathf.Cos(angle) * radiusX;
            float z = Mathf.Sin(angle) * radiusZ;
            nextPos = center + new Vector3(x, 0, z);
        }

        // Behold oprindelig Y-højde (så de ikke hopper op/ned)
        nextPos.y = transform.position.y;

        // ───── Flyt stime ─────
        transform.position = nextPos;

        // ───── Rotér så stimen peger i bevægelsesretningen ─────
        Vector3 dir = (nextPos - center).normalized;       // retning væk fra midt
        Vector3 tangent = Vector3.Cross(Vector3.up, dir);  // retning langs cirklen
        if (tangent.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
    }
}
