using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string nextSceneName;
    public string previousSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Load next scene
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);

            // Unload previous scene
            if (!string.IsNullOrEmpty(previousSceneName))
                SceneManager.UnloadSceneAsync(previousSceneName);
        }
    }
}

        
    }
}
