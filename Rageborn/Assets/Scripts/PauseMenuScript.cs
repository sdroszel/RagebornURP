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
        // Explicitly ensure the game starts unpaused
        isGamePaused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);

        // Initialize buttons
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
            Debug.Log("Escape pressed. Toggling pause...");
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
        Debug.Log("TogglePause called. Current isGamePaused state: " + isGamePaused);

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
        Debug.Log("Resuming game...");
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isGamePaused = false;
        pauseMenu.SetActive(false);
    }

    private void PauseGame()
    {
        Debug.Log("Pausing game... Step 1: Setting Cursor.lockState to None");
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Pausing game... Step 2: Setting Cursor.visible to true");
        Cursor.visible = true;

        Debug.Log("Pausing game... Step 3: Setting Time.timeScale to 0");
        Time.timeScale = 0;

        Debug.Log("Pausing game... Step 4: Setting isGamePaused to true");
        isGamePaused = true;

        Debug.Log("Pausing game... Step 5: Activating pauseMenu");
        pauseMenu.SetActive(true);

        Debug.Log("Pausing game... Completed.");
    }

}
