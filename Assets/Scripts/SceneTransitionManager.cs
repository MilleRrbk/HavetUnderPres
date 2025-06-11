using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    [System.Serializable]
    public class FogSettings
    {
        public Color fogColor = Color.gray;
        public float fogDensity = 0.01f;
    }

    public FogSettings fortidsFog;
    public FogSettings nutidsFog;
    public FogSettings fremtidsFog;

    [Header("XR Rig Positioning")]
    public Vector3 xrRigTargetPosition = new Vector3(0, 1.6f, 0); // 👈 You control this in the Inspector

    private string currentScene = "FortidsScene";

    private void Start()
    {
        RenderSettings.fog = true;
        ApplyFogSettings(fortidsFog);

        SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive).completed += (op) =>
        {
            Debug.Log($"✅ {currentScene} successfully loaded.");
            AlignXRRigToInspectorPosition(); // 👈 use inspector-defined position
        };
    }

    public void LoadNextScene()
    {
        StartCoroutine(TransitionToNextScene());
    }

    private IEnumerator TransitionToNextScene()
    {
        yield return StartCoroutine(FogFade(RenderSettings.fogDensity, 0.1f, 1f));

        string nextScene = GetNextScene(currentScene);
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.Log("Ingen flere scener.");
            yield break;
        }

        yield return SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(currentScene);

        currentScene = nextScene;
        ApplyFogSettings(GetFogForScene(currentScene));

        yield return StartCoroutine(FogFade(0.1f, RenderSettings.fogDensity, 1f));

        AlignXRRigToInspectorPosition(); // 👈 reposition XR rig again
    }

    private IEnumerator FogFade(float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            RenderSettings.fogDensity = Mathf.Lerp(from, to, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        RenderSettings.fogDensity = to;
    }

    private void ApplyFogSettings(FogSettings settings)
    {
        RenderSettings.fogColor = settings.fogColor;
        RenderSettings.fogDensity = settings.fogDensity;
    }

    private string GetNextScene(string scene)
    {
        switch (scene)
        {
            case "FortidsScene": return "NutidsScene";
            case "NutidsScene": return "FremtidsScene";
            default: return null;
        }
    }

    private FogSettings GetFogForScene(string scene)
    {
        switch (scene)
        {
            case "FortidsScene": return fortidsFog;
            case "NutidsScene": return nutidsFog;
            case "FremtidsScene": return fremtidsFog;
            default: return new FogSettings();
        }
    }

    // ✅ This is the new inspector-based rig positioning
    private void AlignXRRigToInspectorPosition()
    {
        GameObject xrRig = GameObject.Find("XR Origin");
        if (xrRig != null)
        {
            xrRig.transform.position = xrRigTargetPosition;
            Debug.Log($"🟢 XR Rig moved to {xrRigTargetPosition}");
        }
        else
        {
            Debug.LogWarning("⚠ XR Origin not found in scene.");
        }
    }
}

