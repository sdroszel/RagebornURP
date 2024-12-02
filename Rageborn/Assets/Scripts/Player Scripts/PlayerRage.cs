using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerRage : MonoBehaviour
{
    [Header("Rage Settings")]
    [SerializeField] private float maxRage = 70f;    // Maximum Rage value
    [SerializeField] private float currentRage = 0f;  // Current Rage value
    [SerializeField] private Image rageBarFill;       // Image to represent the Rage bar
    [SerializeField] private ParticleSystem rageState;
    [SerializeField] GameObject rageFullText;
    [SerializeField] AudioClip rageActivatedAudio;

    [Header("Rage Activation Settings")]
    [SerializeField] private float rageActivationThreshold = 70f; // Threshold for activating rage
    [SerializeField] private float rageDrainSpeed = 7f;  // How much rage drains per second when activated

    private bool isRageActive = false;
    private Coroutine rageDrainCoroutine;  // Reference to the running coroutine

    private PlayerAudio playerAudio;

    private void Start()
    {
        rageState.Stop();
        rageFullText.gameObject.SetActive(false);

        playerAudio = GetComponent<PlayerAudio>();
    }

    private void Update()
    {
        if (IsRageFull())
        {
            rageFullText.gameObject.SetActive(true);
        }
        else
        {
            rageFullText.gameObject.SetActive(false);
        }
    }

    // Method to add rage
    public void AddRage(float amount)
    {
        if (isRageActive)
        {
            return;
        }

        currentRage += amount;
        currentRage = Mathf.Clamp(currentRage, 0, maxRage);  // Ensure rage stays within the max rage limit
        UpdateRageBar();
    }

    // Method to update the rage bar UI
    private void UpdateRageBar()
    {
        if (rageBarFill != null)
        {
            rageBarFill.fillAmount = currentRage / maxRage;  // Update the fill amount based on current rage
        }
    }

    // Method to check if the rage bar is full (for passive effects)
    public bool IsRageFull()
    {
        return currentRage >= maxRage;  // Returns true if the rage bar is full
    }

    // Method to check if rage is active
    public bool IsRageActive()
    {
        return isRageActive;  // Returns whether rage is active
    }

    // Activate rage, providing buffs when rage is full
    public void ActivateRage()
    {
        if (!IsRageFull()) return;  // Don't activate if rage isn't full

        if (isRageActive) return;  // Don't activate if rage is already active

        isRageActive = true;

        rageState.Play();

        playerAudio.AudioSource.PlayOneShot(rageActivatedAudio);

        // Start the drain coroutine when rage is activated
        rageDrainCoroutine = StartCoroutine(DrainRageOverTime());

        // Example: Apply buffs (you can modify the PlayerController logic here)
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.playerMovement.CurrentMoveSpeed *= 1.10f;  // Increase movement speed by 10%
            playerController.playerCombat.IncreaseDamage(1.50f);  // Example: Increase damage by 50%
        }
    }

    // Coroutine to drain rage over time
    private IEnumerator DrainRageOverTime()
    {
        while (isRageActive && currentRage > 0)
        {
            if (PauseMenuScript.isGamePaused)
            {
                yield return new WaitUntil(() => !PauseMenuScript.isGamePaused);  // Wait until the game is unpaused
            }
            currentRage -= rageDrainSpeed;  // Drain a fixed amount of rage per second
            currentRage = Mathf.Clamp(currentRage, 0, maxRage);  // Ensure rage doesn't go below 0
            UpdateRageBar();  // Update the rage bar as it drains

            if (currentRage <= 0)
            {
                StartCoroutine(DeactivateRage());  // Stop rage when it reaches 0
            }

            yield return new WaitForSeconds(1f);  // Wait for 1 second between each drain
        }
    }

    // Deactivate rage when it ends or is manually stopped
    public IEnumerator DeactivateRage()
    {
        yield return new WaitForSeconds(0.5f);

        if (!isRageActive) yield break;

        isRageActive = false;

        rageState.Stop();

        // Stop the coroutine if it's running
        if (rageDrainCoroutine != null)
        {
            StopCoroutine(rageDrainCoroutine);
        }

        // Reset any applied buffs (example)
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.playerMovement.CurrentMoveSpeed /= 1.10f;  // Reset movement speed
            playerController.playerCombat.IncreaseDamage(1f);  // Reset damage multiplier to default
        }

        Debug.Log("Rage Deactivated!");  // Debug Log to confirm deactivation
    }

    // Reset rage (e.g., on death or other conditions)
    public void ResetRage()
    {
        currentRage = 0;
        UpdateRageBar();
    }
}
