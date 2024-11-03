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
        Time.timeScale = 1;
        isGamePaused = false;
        pauseMenu.SetActive(false);
    }
}
