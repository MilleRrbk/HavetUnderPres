using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    public string nextScene;
    public Color fogColor = Color.blue;
    public float fogDensity = 0.01f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.TransitionToScene(nextScene, fogColor, fogDensity);
        }
    }
}