using UnityEngine;

public class ProximityLoader : MonoBehaviour
{
    public Transform player;        // Spilleren eller kameraets position
    public float loadRadius = 50f;  // Hvor tæt modellen skal være for at blive aktiveret

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

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