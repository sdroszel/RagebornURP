using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxSprintDuration = 5f;
    [SerializeField] private float sprintResumeThreshold = 1f;
    [SerializeField] private Image sprintBarFill;
    private float currentSprintDuration;
    private bool sprintDisabled;

    private void Awake()
    {
        currentSprintDuration = maxSprintDuration;
    }

    private void Update()
    {
        if (sprintBarFill != null)
        {
            sprintBarFill.fillAmount = currentSprintDuration / maxSprintDuration;
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

    public void ReplenishSprint()
    {
        if (currentSprintDuration < maxSprintDuration)
        {
            currentSprintDuration += Time.deltaTime;
            currentSprintDuration = Mathf.Min(currentSprintDuration, maxSprintDuration);

            if (currentSprintDuration >= sprintResumeThreshold)
            {
                sprintDisabled = false;
            }
        }
    }
}
