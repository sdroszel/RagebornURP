using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToggleBGMusic() {
        bool isPlaying = backgroundMusic.isPlaying;

        if (isPlaying) {
            backgroundMusic.Stop();
        }
        else {
            backgroundMusic.Play();
        }
    }
}
