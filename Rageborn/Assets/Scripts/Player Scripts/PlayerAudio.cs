using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This class handles the player audio components
/// </summary>
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip runningSnow;
    [SerializeField] private AudioClip runningFloor;
    [SerializeField] private float footstepDelay;

    private bool isDungeonFloor = false;
    private bool isFootstepsPaused = false;

    private PlayerController playerController;
    private AudioSource audioSource;

    public AudioSource AudioSource => audioSource;

    // Awake function gets the needed components
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();
    }

    // Update function called every frame
    private void Update() 
    {
        // Stop footstep audio if player is dead or game is paused
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) 
        {
            StopFootsteps();
        }
    }

    // Detects collision between player and ground to determine the footstep audio
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Floor"))
        {
            bool newFloorType = true;

            if (isDungeonFloor != newFloorType)
            {
                isDungeonFloor = newFloorType;

                SetFootstepAudio();
            }
        }
        else
        {
            if (isDungeonFloor)
            {
                isDungeonFloor = false;

                SetFootstepAudio();
            }
        }
    }

    // This sets the correct footstep audio clip
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

    // Function to stop the footstep audio
    public void StopFootsteps()
    {
        isFootstepsPaused = true;
        audioSource.Stop();
        audioSource.clip = null;
    }

    // Handles resuming footstep audio
    public void ResumeFootsteps()
    {
        StartCoroutine(FootstepDelay());
    }

    // Delays footstep audio before playing
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

    // Used to get the current audio clip
    public AudioClip GetCurrentRunningSound()
    {
        return isDungeonFloor ? runningFloor : runningSnow;
    }
}
