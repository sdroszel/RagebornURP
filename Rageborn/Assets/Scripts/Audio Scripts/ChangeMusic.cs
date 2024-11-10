using UnityEngine;

// Used to change music
public class ChangeMusic : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    void Awake()
    {
        AudioManager.instance.ChangeMusic(audioClip);
    }
}
