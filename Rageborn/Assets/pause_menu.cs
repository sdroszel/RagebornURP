using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuScript : MonoBehaviour
{
    bool gamePaused = false;
    [SerializeField] GameObject pauseMenu;

    public void Home()
    {
        SceneManager.LoadScene(0);
    }

    public void pause()
    {
        Time.timeScale = 0;
        gamePaused = true;
        pauseMenu.SetActive(true);

    }

    public void Continue()
    {
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
    }
}
