using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [System.Serializable]
    public class FogSettings
    {
        public Color fogColor;
        public float fogDensity;
    }

    public FogSettings fortidsFog;
    public FogSettings nutidsFog;
    public FogSettings fremtidsFog;

    private string currentSceneName = "FortidsScene";

    void Start()
    {
        RenderSettings.fog = true;
        ApplyFogSettings(fortidsFog);
        SceneManager.LoadSceneAsync("FortidsScene", LoadSceneMode.Additive);
    }


    // Kald denne fra en trigger fx
    public void LoadNextScene()
    {
        StartCoroutine(TransitionToNextScene());
    }

    private IEnumerator TransitionToNextScene()
    {
        // Fog fade in (increase density)
        yield return StartCoroutine(FogFade(0f, 0.1f, 1f)); // fade til en neutral fade density for overgang

        string nextScene = GetNextSceneName(currentSceneName);

        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.Log("Ingen flere scener!");
            yield break;
        }

        // Load next scene additive
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Unload current scene
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneName);
        while (!unloadOp.isDone)
            yield return null;

        currentSceneName = nextScene;

        // Apply fog settings for new scene
        FogSettings fogSettings = GetFogSettingsForScene(currentSceneName);
        ApplyFogSettings(fogSettings);

        // Fog fade out (reduce density)
        yield return StartCoroutine(FogFade(0.1f, fogSettings.fogDensity, 1f));
    }

    private IEnumerator FogFade(float startDensity, float endDensity, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, endDensity, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        RenderSettings.fogDensity = endDensity;
    }

    private void ApplyFogSettings(FogSettings settings)
    {
        RenderSettings.fogColor = settings.fogColor;
        RenderSettings.fogDensity = settings.fogDensity;
    }

    private string GetNextSceneName(string current)
    {
        switch (current)
        {
            case "FortidsScene": return "NutidsScene";
            case "NutidsScene": return "FremtidsScene";
            default: return null;
        }
    }

    private FogSettings GetFogSettingsForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "FortidsScene": return fortidsFog;
            case "NutidsScene": return nutidsFog;
            case "FremtidsScene": return fremtidsFog;
            default: return new FogSettings { fogColor = Color.gray, fogDensity = 0.01f };
        }
    }
}

