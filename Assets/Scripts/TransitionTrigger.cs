using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    public SceneTransitionManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            manager.LoadNextScene();
            // Disable trigger så det kun sker én gang
            gameObject.SetActive(false);
        }
    }
}

