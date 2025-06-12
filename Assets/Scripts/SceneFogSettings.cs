using UnityEngine;

public class SceneFogSettings : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;
    public FogMode fogMode = FogMode.Exponential;

    [Header("Skybox Settings")]
    public Material skyboxMaterial;

    public void ApplyNow()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogMode = fogMode;

        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            DynamicGI.UpdateEnvironment();
        }
    }

    private void Start()
    {
        if (gameObject.scene == UnityEngine.SceneManagement.SceneManager.GetActiveScene())
        {
            ApplyNow();
        }
    }
}


    
    
