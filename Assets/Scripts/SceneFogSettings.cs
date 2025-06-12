using UnityEngine;

public class SceneFogSettings : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;
    public FogMode fogMode = FogMode.Exponential;

    [Header("Optional Skybox")]
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
            DynamicGI.UpdateEnvironment(); // Opdater lyset efter skyboxændring
        }
    }

    void Start()
    {
        ApplyNow();
    }
}

    
    
