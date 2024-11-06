using UnityEngine;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public static bool isGamePaused = false;
    [SerializeField] private GameObject pauseMenu;

    private Button continueButton;
    private Button mainMenuButton;

    private void Start()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);

        pauseMenu.SetActive(true);
        continueButton = pauseMenu.transform.Find("ContinueButton").GetComponent<Button>();
        mainMenuButton = pauseMenu.transform.Find("MainMenuButton").GetComponent<Button>();

        continueButton.onClick.AddListener(Continue);
        mainMenuButton.onClick.AddListener(MainMenu);
        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManagerScript.instance.LoadMainMenu();
    }

    public void Continue()
    {
        ResumeGame();
    }

    private void TogglePause()
    {

        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGamePaused = false;
        pauseMenu.SetActive(false);
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0;

        isGamePaused = true;

        pauseMenu.SetActive(true);

    }

}
