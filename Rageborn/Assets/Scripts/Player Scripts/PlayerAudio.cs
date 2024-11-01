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
    private bool isFootstepsPaused = false; // Track if footsteps are paused

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
        // Ensure footsteps play only if they are not paused
        if (!isFootstepsPaused)
        {
            audioSource.clip = isDungeonFloor ? runningFloor : runningSnow;
            if (!audioSource.isPlaying)
            {
                audioSource.loop = true; // Enable looping for continuous steps
                audioSource.Play();
            }
        }
    }

    public void StopFootsteps()
    {
        isFootstepsPaused = true;
        audioSource.Stop(); // Stop immediately
        audioSource.clip = null; // Clear the clip to ensure no sound plays accidentally
    }

    public void ResumeFootsteps()
    {
        StartCoroutine(FootstepDelay());
    }

    private IEnumerator FootstepDelay()
    {
        yield return new WaitForSeconds(footstepDelay);

        isFootstepsPaused = false;

        // Set clip based on floor type and play if moving
        audioSource.clip = isDungeonFloor ? runningFloor : runningSnow;
        if (audioSource.clip != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Expose AudioSource to other components, such as PlayerMovement
    public AudioSource AudioSource => audioSource;

    // Provide the current footstep sound based on the floor type
    public AudioClip GetCurrentRunningSound()
    {
        return isDungeonFloor ? runningFloor : runningSnow;
    }
}
