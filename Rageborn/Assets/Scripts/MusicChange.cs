using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChange : MonoBehaviour
{
[SerializeField] private AudioClip audioClip;

    private void Awake() {
        AudioManager.instance.ChangeMusic(audioClip);
    }
}
