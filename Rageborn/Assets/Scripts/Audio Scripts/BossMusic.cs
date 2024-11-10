using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMusic : MonoBehaviour
{
    [SerializeField] private AudioClip bossMusic;
    
    private void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.ChangeMusic(bossMusic);
    }
}
