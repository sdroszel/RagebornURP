using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public bool isPlaying = true;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float loopFade = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // Method to toggle music on/off based on Toggle's state
    public void ToggleMusic(bool isOn)
    {
        isPlaying = isOn;

        if (isPlaying)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
                musicSource.volume = 0.5f; // Ensure volume is set correctly
            }
        }
        else
        {
            musicSource.Stop(); // Stop playback entirely
        }
    }
}
