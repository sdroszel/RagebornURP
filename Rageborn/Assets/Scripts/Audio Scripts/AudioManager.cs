using System.Collections;
using UnityEngine;

/// <summary>
/// This class handles the music in the game.
/// It uses the singleton pattern and doNotDestroyOnLoad
/// to make sure that it is the only one that exists between scenes.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float loopFade = 0.5f;

    public static AudioManager instance;
    public bool isPlaying = true;

    // Creates new audioManager if none exists and presists through scene changes
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Makes sure only one exists
            Destroy(gameObject);
        }
    }

    // Starts playing the background music
    void Start()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // Toggles the music on or off
    public void ToggleMusic(bool isOn)
    {
        isPlaying = isOn;

        if (isPlaying)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
                musicSource.volume = 0.25f;
            }
        }
        else
        {
            musicSource.Stop();
        }
    }

    // Used to change the background music in other levels
    public void ChangeMusic(AudioClip newClip)
    {
        StartCoroutine(FadeOutAndChangeClip(newClip));
    }

    // Fades the music out when changing tracks
    private IEnumerator FadeOutAndChangeClip(AudioClip newClip)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        
        musicSource.clip = newClip;

        if (isPlaying)
        {
            musicSource.Play();
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, 0.25f, t / fadeDuration);
                yield return null;
            }
        }
        else
        {
            musicSource.volume = 0.25f;
        }
    }
}
