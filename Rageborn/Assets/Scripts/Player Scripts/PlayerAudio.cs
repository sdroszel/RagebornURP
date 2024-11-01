using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip runningSnow;
    [SerializeField] private AudioClip runningFloor;
    private AudioSource audioSource;
    private bool isDungeonFloor = false;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Floor")) {
            isDungeonFloor = true;
            SetFootstepAudio();
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Floor")) {
            isDungeonFloor = false;
            SetFootstepAudio();
        }
    }

    private void SetFootstepAudio() {
        audioSource.clip = isDungeonFloor ? runningFloor : runningSnow;
        if (!audioSource.isPlaying) audioSource.Play();
    }

    // Expose AudioSource to other components, such as PlayerMovement
    public AudioSource AudioSource => audioSource;

    // Provide the current footstep sound based on the floor type
    public AudioClip GetCurrentRunningSound() {
        return isDungeonFloor ? runningFloor : runningSnow;
    }
}
