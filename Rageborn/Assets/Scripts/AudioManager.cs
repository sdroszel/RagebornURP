using System.Collections;
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

    public void ToggleMusic(bool isOn)
    {
        isPlaying = isOn;

        if (isPlaying)
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
                musicSource.volume = 0.5f;
            }
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        StartCoroutine(FadeOutAndChangeClip(newClip));
    }

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
                musicSource.volume = Mathf.Lerp(0, 0.5f, t / fadeDuration);
                yield return null;
            }
        }
        else
        {
            musicSource.volume = 0.5f;
        }
    }
}
