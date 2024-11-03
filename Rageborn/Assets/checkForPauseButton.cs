using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkForPauseButton : MonoBehaviour
{

    public bool gamePaused = false;
    [SerializeField] GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && pauseMenu.activeSelf == false)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else if ((Input.GetKeyDown(KeyCode.Tab) && pauseMenu.activeSelf == true))
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
