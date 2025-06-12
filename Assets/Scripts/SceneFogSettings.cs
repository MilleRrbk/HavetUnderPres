using UnityEngine;

public class SceneFogSettings : MonoBehaviour
{
    [Header("Fog Settings")]
    public bool enableFog = true;
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;
    public FogMode fogMode = FogMode.Exponential;

    void Start()
    {
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.fogMode = fogMode;
    }
    
}
    
    
