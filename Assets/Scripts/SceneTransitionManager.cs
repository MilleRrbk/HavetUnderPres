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

    private string currentScene = "FortidsScene";

    private void Start()
    {
        RenderSettings.fog = true;
        ApplyFogSettings(fortidsFog);

        // Load first scene additively and wait for it to finish
        SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive).completed += (op) =>
        {
            Debug.Log($"✅ {currentScene} successfully loaded.");
            AlignXRRigToSceneOrigin();
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

        AlignXRRigToSceneOrigin();
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

    // ✅ New helper to reposition XR Rig
    private void AlignXRRigToSceneOrigin()
    {
        GameObject xrRig = GameObject.Find("XR Origin");
        if (xrRig != null)
        {
            Transform cameraOffset = xrRig.transform.Find("Camera Offset");

            if (cameraOffset != null)
            {
                // Move the whole rig so the "Camera Offset" sits at world Y = 3
                Vector3 headsetPos = cameraOffset.position;
                float heightAdjustment = 3f - headsetPos.y;
                xrRig.transform.position += new Vector3(0, heightAdjustment, 0);

                Debug.Log($"🟢 XR Rig repositioned to Y = 3");
            }
            else
            {
                Debug.LogWarning("⚠ Could not find Camera Offset in XR Origin");
            }
        }
        else
        {
            Debug.LogWarning("⚠ XR Origin not found in scene.");
        }
    }

}
