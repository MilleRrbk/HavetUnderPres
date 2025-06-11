using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Tåge Overgang")]
    public float fogFadeDuration = 2f;

    [Header("Audio")]
    public AudioSource backgroundMusic;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.cyan;
        RenderSettings.fogDensity = 0.01f;

        StartCoroutine(LoadSceneAdditive("FortidsScene"));
    }

    public void TransitionToScene(string sceneName, Color nextFogColor, float nextFogDensity)
    {
        if (!isTransitioning)
        {
            Debug.Log("🔁 Starter overgang til: " + sceneName);
            StartCoroutine(DoTransition(sceneName, nextFogColor, nextFogDensity));
        }
        else
        {
            Debug.Log("⚠️ Overgang er allerede i gang.");
        }
    }

    private IEnumerator DoTransition(string nextScene, Color nextFogColor, float nextFogDensity)
    {
        RenderSettings.fog = true;
        isTransitioning = true;

        float t = 0f;
        float startDensity = RenderSettings.fogDensity;
        Color startColor = RenderSettings.fogColor;

        // Fade til sort tåge
        while (t < fogFadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fogFadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, 0.2f, lerp);
            RenderSettings.fogColor = Color.Lerp(startColor, Color.black, lerp);
            yield return null;
        }

        // Fjern alle aktive additive scener (bortset fra MainScene)
        for (int i = 1; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "MainScene")
            {
                Debug.Log("🗑 Unloader scene: " + scene.name);
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        // Load næste scene additive
        Debug.Log("📦 Loader scene: " + nextScene);
        yield return SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);

        // Start med sort tåge igen
        RenderSettings.fogColor = Color.black;
        RenderSettings.fogDensity = 0.2f;

        // Fade ind i den nye tågefarve
        t = 0f;
        while (t < fogFadeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / fogFadeDuration;
            RenderSettings.fogDensity = Mathf.Lerp(0.2f, nextFogDensity, lerp);
            RenderSettings.fogColor = Color.Lerp(Color.black, nextFogColor, lerp);
            yield return null;
        }

        isTransitioning = false;
        Debug.Log("✅ Færdig med overgang til: " + nextScene);
    }

    private IEnumerator LoadSceneAdditive(string sceneName)
    {
        Debug.Log("📥 Loader startscene: " + sceneName);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
