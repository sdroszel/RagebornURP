using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerRage : MonoBehaviour
{
    [Header("Rage Settings")]
    [SerializeField] private float maxRage = 75f;    // Maximum Rage value
    [SerializeField] private float currentRage = 0f;  // Current Rage value
    [SerializeField] private Image rageBarFill;       // Image to represent the Rage bar

    [Header("Rage Activation Settings")]
    [SerializeField] private float rageActivationThreshold = 75f; // Threshold for activating rage
    [SerializeField] private float rageDrainSpeed = 2f;  // How fast rage drains per second when activated

    private bool isRageActive = false;
    private Coroutine rageDrainCoroutine;  // Reference to the running coroutine

    // Method to add rage
    public void AddRage(float amount)
    {
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

    // Optional method to activate rage, providing buffs when rage is full
    public void ActivateRage()
    {
        if (!IsRageFull()) return;  // Don't activate if rage isn't full

        if (isRageActive) return;  // Don't activate if rage is already active

        isRageActive = true;
        Debug.Log("Rage Activated!");  // Debug Log to confirm activation

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
            currentRage -= rageDrainSpeed * Time.deltaTime;  // Drain rage based on the drain speed
            currentRage = Mathf.Clamp(currentRage, 0, maxRage);  // Ensure rage doesn't go below 0
            UpdateRageBar();  // Update the rage bar as it drains
            yield return null;  // Wait until the next frame
        }
    }

    // Optional method to reset rage (e.g., when the player dies)
    public void ResetRage()
    {
        currentRage = 0;
        UpdateRageBar();
    }
}