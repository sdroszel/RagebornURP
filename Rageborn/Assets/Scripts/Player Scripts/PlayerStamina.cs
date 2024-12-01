using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles the player stamina
/// </summary>
public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float sprintResumeThreshold = 1f;
    [SerializeField] private Image staminaBarFill;
    [SerializeField] private ParticleSystem staminaEffect;
    [SerializeField] private float replenishRate = 0.5f;
    [SerializeField] private float replenishAmount = 1f;
    [SerializeField] private AudioClip staminaAudio;
    [SerializeField] private AudioSource audioSource;

    [Header("Potion Cooldown Settings")]
    [SerializeField] private float potionCooldown = 5f;
    [SerializeField] private TextMeshProUGUI potionCooldownText;
    [SerializeField] private Image cooldownBG;

    private bool isPotionOnCooldown = false;
    private bool sprintDisabled;

    private float currentStaminaAmount;

    // Sets UI state
    private void Awake()
    {
        currentStaminaAmount = maxStamina;
        potionCooldownText.text = "";
        cooldownBG.gameObject.SetActive(false);
    }

    // Updates the player stamina every frame
    private void Update()
    {
        if (staminaBarFill != null)
        {
            PotionStamina();
            staminaBarFill.fillAmount = currentStaminaAmount / maxStamina;
        }
    }

    // Used to check if player can sprint
    public bool CanSprint()
    {
        return currentStaminaAmount > 0 && !sprintDisabled;
    }

    // Used to consume stamina while sprinting
    public void ConsumeSprint()
    {
        currentStaminaAmount -= Time.deltaTime;
        currentStaminaAmount = Mathf.Max(currentStaminaAmount, 0f);

        if (currentStaminaAmount <= 0)
        {
            sprintDisabled = true;
        }
    }

    // Used to consume a set amount of stamina
    public void ConsumeStamina(float amount)
    {
        currentStaminaAmount -= amount;
        currentStaminaAmount = Mathf.Max(currentStaminaAmount, 0f);

        if (currentStaminaAmount <= 0)
        {
            sprintDisabled = true;
        }
    }

    // Checks if can consume a set amount of stamina
    public bool CanConsumeStamina(float amount)
    {
        return currentStaminaAmount >= amount;
    }

    // Replenishes stamina over time
    public void ReplenishStamina()
    {
        if (currentStaminaAmount < maxStamina)
        {
            currentStaminaAmount += Time.deltaTime * replenishRate;
            currentStaminaAmount = Mathf.Min(currentStaminaAmount, maxStamina);

            if (currentStaminaAmount >= sprintResumeThreshold)
            {
                sprintDisabled = false;
            }
        }
    }

    // Handles stamina potion use
    private void PotionStamina()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) && !isPotionOnCooldown)
        {
            if (SceneManagerScript.instance.numOfStaminaPotions > 0 && currentStaminaAmount != maxStamina)
            {
                StartCoroutine(IncreaseStamina(replenishAmount));
                StartCoroutine(PotionCooldown());
            }
        }
    }

    // Replenishes a set amount of stamina
    private IEnumerator IncreaseStamina(float replenishAmount)
    {
        audioSource.PlayOneShot(staminaAudio);

        currentStaminaAmount += replenishAmount;

        // Decrement number of stamina potions in scene manager
        SceneManagerScript.instance.numOfStaminaPotions -= 1;

        staminaEffect.Play();

        yield return new WaitForSeconds(0.75f);

        staminaEffect.Stop();
    }

    // Handles stamina potion cooldown
    private IEnumerator PotionCooldown()
    {
        cooldownBG.gameObject.SetActive(true);

        isPotionOnCooldown = true;

        float elapsedTime = 0f;

        while (elapsedTime < potionCooldown)
        {
            // Updates cooldown UI timer
            elapsedTime += Time.deltaTime;
            float remainingTime = potionCooldown - elapsedTime;
            potionCooldownText.text = Mathf.Ceil(remainingTime).ToString();

            yield return null;
        }

        // Reset UI elements
        potionCooldownText.text = "";
        isPotionOnCooldown = false;
        cooldownBG.gameObject.SetActive(false);
    }
}
