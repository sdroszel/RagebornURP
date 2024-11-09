using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
    private float currentSprintDuration;
    private bool sprintDisabled;

    private void Awake()
    {
        currentSprintDuration = maxStamina;
        potionCooldownText.text = "";
        cooldownBG.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (staminaBarFill != null)
        {
            PotionStamina();
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

    private void PotionStamina()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isPotionOnCooldown)
        {
            if (SceneManagerScript.instance.numOfStaminaPotions > 0 && currentSprintDuration != maxStamina)
            {
                StartCoroutine(IncreaseStamina(replenishAmount));
                StartCoroutine(PotionCooldown());
            }
        }
    }

    private IEnumerator IncreaseStamina(float replenishAmount)
    {
        audioSource.PlayOneShot(staminaAudio);
        currentSprintDuration += replenishAmount;
        SceneManagerScript.instance.numOfStaminaPotions -= 1;
        staminaEffect.Play();

        yield return new WaitForSeconds(0.75f);

        staminaEffect.Stop();
    }

    private IEnumerator PotionCooldown()
    {
        cooldownBG.gameObject.SetActive(true);
        isPotionOnCooldown = true;
        float elapsedTime = 0f;

        while (elapsedTime < potionCooldown)
        {
            elapsedTime += Time.deltaTime;
            float remainingTime = potionCooldown - elapsedTime;
            potionCooldownText.text = Mathf.Ceil(remainingTime).ToString();

            yield return null;
        }

        potionCooldownText.text = "";
        isPotionOnCooldown = false;
        cooldownBG.gameObject.SetActive(false);
    }
}
