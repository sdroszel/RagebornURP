using System.Collections;
using UnityEngine;

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

    void Update()
    {
        if (musicSource.isPlaying && musicSource.time >= musicSource.clip.length - loopFade)
        {
            StartCoroutine(HandleLoop());
        }
    }

    private IEnumerator HandleLoop()
    {
        yield return StartCoroutine(FadeOut());

        // Restart the music from the beginning
        musicSource.time = 0;
        musicSource.Play();

        yield return StartCoroutine(FadeIn());
    }

    // Method to change the music track
    public void ChangeMusic(AudioClip newClip)
    {
        StartCoroutine(FadeOutAndChangeMusic(newClip));
    }

    private IEnumerator FadeOutAndChangeMusic(AudioClip newClip)
    {
        // Fade out the current music
        yield return StartCoroutine(FadeOut());

        if (musicSource.isPlaying) {
        
            // Change the clip and play it
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in the new music
            yield return StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeOut()
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0;
    }

    private IEnumerator FadeIn()
    {
        float startVolume = 0.2f;
        musicSource.volume = startVolume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0.5f, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = 0.5f;
    }

    public void ToggleMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            isPlaying = false;
        }
        else
        {
            musicSource.time = 0;
            musicSource.UnPause();
            isPlaying = true;
        }
    }

}
