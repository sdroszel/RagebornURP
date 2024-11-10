using UnityEngine;

// Handles changing music for boss room
public class BossMusic : MonoBehaviour
{
    [SerializeField] private AudioClip bossMusic;
    
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.ChangeMusic(bossMusic);
    }
}
