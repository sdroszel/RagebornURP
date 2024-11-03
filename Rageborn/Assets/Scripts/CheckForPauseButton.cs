using UnityEngine;

public class CheckForPauseButton : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && pauseMenu.activeSelf == false)
        {
            PauseMenuScript.isGamePaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && pauseMenu.activeSelf == true)
        {
            PauseMenuScript.isGamePaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
