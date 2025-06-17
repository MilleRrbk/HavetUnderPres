using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Canvas))]
public class CanvasSetupTool : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("🔧 Gør Canvas synlig i VR (foran alt)"))
        {
            SetupCanvas((Canvas)target);
        }
    }

    private void SetupCanvas(Canvas canvas)
    {
        // Skift til Screen Space - Camera
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        // Sæt main camera som renderkamera
        if (Camera.main != null)
            canvas.worldCamera = Camera.main;

        // Sæt override sorting
        canvas.overrideSorting = true;

        // Prøv at sætte sorting layer til "UI"
        try
        {
            canvas.sortingLayerName = "UI";
        }
        catch
        {
            Debug.LogWarning("Sorting layer 'UI' ikke fundet – opret det manuelt i Project Settings → Tags and Layers.");
        }

        // Høj prioritet
        canvas.sortingOrder = 500;

        Debug.Log("✅ Canvas sat korrekt op til VR visning.");
    }
} 