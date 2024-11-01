using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxSprintDuration = 5f;
    [SerializeField] private float sprintResumeThreshold = 1f; // Stamina required to resume sprinting
    [SerializeField] private Image sprintBarFill;
    private float currentSprintDuration;
    private bool sprintDisabled; // Track if sprinting is disabled due to depletion

    private void Awake() {
        currentSprintDuration = maxSprintDuration;
    }

    private void Update() {
        if (sprintBarFill != null) {
            sprintBarFill.fillAmount = currentSprintDuration / maxSprintDuration;
        }
    }

    public bool CanSprint() {
        // Allow sprinting if stamina is above the resume threshold and sprint is not disabled
        return currentSprintDuration > 0 && !sprintDisabled;
    }

    public void ConsumeSprint() {
        // Reduce stamina, allowing it to reach zero
        currentSprintDuration -= Time.deltaTime;
        currentSprintDuration = Mathf.Max(currentSprintDuration, 0f);

        // Disable sprint if stamina is depleted
        if (currentSprintDuration <= 0) {
            sprintDisabled = true;
        }
    }

    public void ReplenishSprint() {
        // Refill stamina when sprinting is inactive
        if (currentSprintDuration < maxSprintDuration) {
            currentSprintDuration += Time.deltaTime;
            currentSprintDuration = Mathf.Min(currentSprintDuration, maxSprintDuration);

            // Re-enable sprinting once stamina reaches the resume threshold
            if (currentSprintDuration >= sprintResumeThreshold) {
                sprintDisabled = false;
            }
        }
    }
}
