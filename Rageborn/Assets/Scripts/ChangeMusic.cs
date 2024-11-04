using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    void Awake()
    {
        AudioManager.instance.ChangeMusic(audioClip);
    }
}
