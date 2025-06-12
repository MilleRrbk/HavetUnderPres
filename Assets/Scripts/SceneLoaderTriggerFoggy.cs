using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderTriggerFoggy : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneToUnload;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        // Load the new scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Optionally unload previous scene
        if (!string.IsNullOrEmpty(sceneToUnload) && SceneManager.GetSceneByName(sceneToUnload).isLoaded)
            SceneManager.UnloadSceneAsync(sceneToUnload);

        // Apply fog from the new scene manually
        Scene activeScene = SceneManager.GetSceneByName(sceneToLoad);
        GameObject[] roots = activeScene.GetRootGameObjects();
        foreach (GameObject root in roots)
        {
            SceneFogSettings fogSettings = root.GetComponentInChildren<SceneFogSettings>();
            if (fogSettings != null)
            {
                fogSettings.ApplyNow();
                break;
            }
        }
    }
}
