using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderTriggerFoggy : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;     // Den scene, der skal loades (fx "Scene_Nutid")
    [SerializeField] private string sceneToUnload;   // Den scene, der skal unloades (fx "Scene_Fortid")

    [Header("Target Fog Settings")]
    public Color targetFogColor = Color.gray;
    public float targetFogDensity = 0.02f;

    [Header("Transition Settings")]
    public float transitionDuration = 2f;
    public float initialFogDensity = 1.0f;

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
        // Start med kraftig tåge (illusion af rejse)
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.white;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = initialFogDensity;

        // Load ny scene additivt
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // LILLE PAUSE før Unload, så det ikke sker for hurtigt
        yield return new WaitForSeconds(0.2f);

        // Unload den gamle scene hvis den stadig er loaded
        if (!string.IsNullOrEmpty(sceneToUnload) &&
            SceneManager.GetSceneByName(sceneToUnload).isLoaded)
        {
            Debug.Log("Unloader scene: " + sceneToUnload);
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(sceneToUnload);
            while (!unloadOp.isDone)
                yield return null;
        }
        else
        {
            Debug.LogWarning("Kunne ikke unloade: " + sceneToUnload);
        }

        // Fade tågen langsomt til målindstillinger
        float elapsed = 0f;
        float startDensity = initialFogDensity;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            RenderSettings.fogColor = Color.Lerp(Color.white, targetFogColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, targetFogDensity, t);
            yield return null;
        }

        // Sæt endelig tåge
        RenderSettings.fogColor = targetFogColor;
        RenderSettings.fogDensity = targetFogDensity;
    }
}
