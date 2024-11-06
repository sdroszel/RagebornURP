using UnityEngine;


public class PauseMenuScript : MonoBehaviour
{
    public static bool isGamePaused = false;
    [SerializeField] GameObject pauseMenu;

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManagerScript.instance.LoadMainMenu();
    }

    public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1;
        isGamePaused = false;
        pauseMenu.SetActive(false);
    }
}
