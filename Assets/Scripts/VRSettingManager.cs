using UnityEngine;
using UnityEngine.XR;

public class LowerVRResolution : MonoBehaviour
{
    [Range(0.1f, 1.5f)]
    public float resolutionScale = 0.50f;

    void Start()
    {
        XRSettings.eyeTextureResolutionScale = resolutionScale;
    }
}
