using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TeleportPromptController : MonoBehaviour
{
    [SerializeField] private GameObject teleportPrompt;
    [SerializeField] private Scene scene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            teleportPrompt.SetActive(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the object has the "Player" tag
        {
            teleportPrompt.SetActive(false); // Hide the teleport prompt
        }
    }

    private void Update()
    {
        if (teleportPrompt.activeSelf && Input.GetKeyDown(KeyCode.E))
        {
            Teleport(); // Call the teleport function when E is pressed
        }
    }

    private void Teleport()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
