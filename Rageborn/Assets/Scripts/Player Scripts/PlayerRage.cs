using UnityEngine;
using UnityEngine.UI;


public class PlayerRage : MonoBehaviour
{
    [Header("Rage Settings")]
    [SerializeField] private float maxRage = 75f;    // Maximum Rage value
    [SerializeField] private float currentRage = 0f;  // Current Rage value
    [SerializeField] private Image rageBarFill;       // Image to represent the Rage bar


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


    // Method to use rage (for example, using special abilities)
    public void UseRage(float amount)
    {
        if (currentRage >= amount)
        {
            currentRage -= amount;
            UpdateRageBar();
        }
    }


    // Optionally, reset rage if needed (e.g., when player dies)
    public void ResetRage()
    {
        currentRage = 0;
        UpdateRageBar();
    }
}


