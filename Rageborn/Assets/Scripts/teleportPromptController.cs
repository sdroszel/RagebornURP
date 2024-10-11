using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Use this if you're using TextMeshPro

public class TeleportPromptController : MonoBehaviour
{
    [SerializeField] private GameObject teleportPrompt; // Reference to the UI Text GameObject
    [SerializeField] private Scene scene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure the object has the "Player" tag
        {
            teleportPrompt.SetActive(true); // Show the teleport prompt
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
