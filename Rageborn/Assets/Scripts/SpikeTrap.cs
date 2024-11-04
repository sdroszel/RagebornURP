using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AudioClip audioClip;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") {
            playerController.playerHealth.TakeDamage(100);
            audioSource.PlayOneShot(audioClip);
        }
    }
}
