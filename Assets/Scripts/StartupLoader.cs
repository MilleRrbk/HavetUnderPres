using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Scene_Fortid";

    void Start()
    {
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }
    }
}

