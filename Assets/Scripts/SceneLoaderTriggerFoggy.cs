using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderTriggerFoggy : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;     // Next scene to load (e.g. "Scene_Nutid")
    [SerializeField] private string sceneToUnload;   // Scene to unload (e.g. "Scene_Fortid")

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(LoadSceneRoutine());
        }
    }

    private IEnumerator LoadSceneRoutine()
    {
        Debug.Log("🔁 Loading scene: " + sceneToLoad);

        // Load new scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Set new scene as active
        Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
        if (newScene.IsValid())
            SceneManager.SetActiveScene(newScene);

        // Apply fog settings from the newly loaded scene
        ApplyFogFromScene(newScene);

        // Optional short delay before unloading old scene
        yield return new WaitForSeconds(0.2f);

        // Unload the previous scene
        if (!string.IsNullOrEmpty(sceneToUnload) && SceneManager.GetSceneByName(sceneToUnload).isLoaded)
        {
            Debug.Log("🧹 Unloading scene: " + sceneToUnload);
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (!unloadOp.isDone)
                yield return null;
        }
        else
        {
            Debug.LogWarning("⚠️ Could not unload: " + sceneToUnload);
        }

        Debug.Log("✅ Scene switch complete.");
    }

    private void ApplyFogFromScene(Scene targetScene)
    {
        foreach (GameObject obj in targetScene.GetRootGameObjects())
        {
            SceneFogSettings fogSettings = obj.GetComponentInChildren<SceneFogSettings>();
            if (fogSettings != null)
            {
                fogSettings.ApplyNow();
                break;
            }
        }
    }
}
