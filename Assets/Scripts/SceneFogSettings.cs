using UnityEngine;

public class SceneFogSettings : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;
    public FogMode fogMode = FogMode.Exponential;

    public void ApplyNow()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    void Start()
    {
        ApplyNow(); // Apply when the scene starts as well
    }
}

    
    
