using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float sprintResumeThreshold = 1f;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private float replenishRate = 0.5f;
    private float currentSprintDuration;
    private bool sprintDisabled;

    private void Awake()
    {
        currentSprintDuration = maxStamina;
    }

    private void Update()
    {
        if (staminaBarFill != null)
        {
            staminaBarFill.fillAmount = currentSprintDuration / maxStamina;
        }
    }

    public bool CanSprint()
    {
        return currentSprintDuration > 0 && !sprintDisabled;
    }

    public void ConsumeSprint()
    {
        currentSprintDuration -= Time.deltaTime;
        currentSprintDuration = Mathf.Max(currentSprintDuration, 0f);

        if (currentSprintDuration <= 0)
        {
            sprintDisabled = true;
        }
    }

    public void ConsumeStamina(float amount)
    {
        currentSprintDuration -= amount;
        currentSprintDuration = Mathf.Max(currentSprintDuration, 0f);

        if (currentSprintDuration <= 0)
        {
            sprintDisabled = true;
        }
    }

    public bool CanConsumeStamina(float amount)
    {
        return currentSprintDuration >= amount;
    }

    public void ReplenishSprint()
    {
        if (currentSprintDuration < maxStamina)
        {
            currentSprintDuration += Time.deltaTime * replenishRate;
            currentSprintDuration = Mathf.Min(currentSprintDuration, maxStamina);

            if (currentSprintDuration >= sprintResumeThreshold)
            {
                sprintDisabled = false;
            }
        }
    }
}
