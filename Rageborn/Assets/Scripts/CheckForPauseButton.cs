using UnityEngine;

public class CheckForPauseButton : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && pauseMenu.activeSelf == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PauseMenuScript.isGamePaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && pauseMenu.activeSelf == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            PauseMenuScript.isGamePaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
