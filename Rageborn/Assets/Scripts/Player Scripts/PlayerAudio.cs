using System;
using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip runningSnow;
    [SerializeField] private AudioClip runningFloor;
    [SerializeField] private float footstepDelay;
    private AudioSource audioSource;
    private bool isDungeonFloor = false;
    private bool isFootstepsPaused = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Floor"))
        {
            isDungeonFloor = true;
            SetFootstepAudio();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Floor"))
        {
            isDungeonFloor = false;
            SetFootstepAudio();
        }
    }

    private void SetFootstepAudio()
    {
        if (!isFootstepsPaused)
        {
            audioSource.clip = isDungeonFloor ? runningFloor : runningSnow;
            if (!audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    public void StopFootsteps()
    {
        isFootstepsPaused = true;
        audioSource.Stop();
        audioSource.clip = null;
    }

    public void ResumeFootsteps()
    {
        StartCoroutine(FootstepDelay());
    }

    private IEnumerator FootstepDelay()
    {
        yield return new WaitForSeconds(footstepDelay);

        isFootstepsPaused = false;

        audioSource.clip = isDungeonFloor ? runningFloor : runningSnow;
        if (audioSource.clip != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public AudioSource AudioSource => audioSource;

    public AudioClip GetCurrentRunningSound()
    {
        return isDungeonFloor ? runningFloor : runningSnow;
    }
}
