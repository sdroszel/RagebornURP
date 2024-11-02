using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (fadeCanvasGroup != null)
        {
            DontDestroyOnLoad(fadeCanvasGroup.gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            AssignMainMenuButtonEvents();
        }
    }

    private void AssignMainMenuButtonEvents()
    {
        Button playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
        Button quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(LoadNextScene);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(FadeAndLoadScene(nextSceneIndex));
        }
    }

    public void LoadMainMenu()
    {
        StartCoroutine(FadeAndLoadScene(0));
    }

    private IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneIndex);
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
    }


    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
