public class ProximityLoader : MonoBehaviour
{
    public Transform vrCamera;       // Hovedets position (Camera)
    public float loadRadius = 50f;   // Afstand hvor objekt aktiveres

    void Update()
    {
        float distance = Vector3.Distance(transform.position, vrCamera.position);

        if (distance <= loadRadius)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }
        else
        {
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
    }
}