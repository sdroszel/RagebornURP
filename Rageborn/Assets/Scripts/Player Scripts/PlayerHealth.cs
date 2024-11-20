using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles the player health components
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float PotionHealAmount = 15f;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private ParticleSystem healEffect;
    [SerializeField] private AudioClip healAudio;
    [SerializeField] private AudioSource audioSource;

    [Header("Potion Cooldown Settings")]
    [SerializeField] private float potionCooldown = 5f;
    [SerializeField] private TextMeshProUGUI potionCooldownText;
    [SerializeField] private Image cooldownBG;

    [Header("Death Settings")]
    [SerializeField] private TextMeshProUGUI gameoverText;
    [SerializeField] private float deathMessageDelay = 2f;

    [Header("Rage Settings")]
    [SerializeField] private float rageOnDamage = 3f; // Amount of rage added when taking damage

    private bool isPotionOnCooldown = false;
    private bool isDead = false;
    private float currentHealth;
    private Animator animator;

    private PlayerRage playerRage; // Reference to the PlayerRage script

    public bool IsDead => isDead;

    // Sets UI state and gets needed components
    private void Awake()
    {
        potionCooldownText.text = "";
        cooldownBG.gameObject.SetActive(false);

        healEffect.Stop();

        // Update current health within scene manager
        currentHealth = SceneManagerScript.instance.playerHealth;

        if (gameoverText != null)
        {
            gameoverText.gameObject.SetActive(false);
        }

        animator = GetComponent<Animator>();

        // Get the PlayerRage component
        playerRage = GetComponent<PlayerRage>();
        if (playerRage == null)
        {
            Debug.LogError("PlayerRage component is missing!");
        }
    }

    // Disables player input
    private void OnDisable()
    {
        var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.Disable();
            playerInput.enabled = false;
        }
    }

    // Updates every frame to check for potion use and updates health bar
    private void Update()
    {
        PotionHeal();
        UpdateHealthBar();
    }

    // Updates health bar
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    // Handles player taking damage
    public void TakeDamage(float damageAmount)
    {
        // Skips if player is dead
        if (isDead) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Updates player health in scene manager
        SceneManagerScript.instance.playerHealth = currentHealth;

        // Add rage when taking damage
        if (playerRage != null)
        {
            playerRage.AddRage(rageOnDamage);
        }

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    // Handles healing player for set amount
    public void Heal(float healAmount)
    {
        // Skip if player is dead
        if (isDead) return;

        StartCoroutine(PerformHeal(healAmount));
    }

    // Handles the healing logic
    private IEnumerator PerformHeal(float healAmount)
    {
        audioSource.PlayOneShot(healAudio);

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Updates player health in scene manager
        SceneManagerScript.instance.playerHealth = currentHealth;

        healEffect.Play();

        yield return new WaitForSeconds(0.5f);

        healEffect.Stop();
    }

    // Listens for key press to use health potion
    private void PotionHeal()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isPotionOnCooldown)
        {
            if (SceneManagerScript.instance.numOfHealthPotions > 0 && currentHealth != maxHealth)
            {
                Heal(PotionHealAmount);

                // Update player health in scene manager
                SceneManagerScript.instance.numOfHealthPotions -= 1;

                StartCoroutine(PotionCooldown());
            }
        }
    }

    // Handles potion use cooldown
    private IEnumerator PotionCooldown()
    {
        cooldownBG.gameObject.SetActive(true);
        isPotionOnCooldown = true;

        // Used to track cooldown time
        float elapsedTime = 0f;

        while (elapsedTime < potionCooldown)
        {
            // Updates the UI countdown text
            elapsedTime += Time.deltaTime;
            float remainingTime = potionCooldown - elapsedTime;
            potionCooldownText.text = Mathf.Ceil(remainingTime).ToString();

            yield return null;
        }

        // Reset cooldown UI elements
        potionCooldownText.text = "";
        isPotionOnCooldown = false;
        cooldownBG.gameObject.SetActive(false);
    }

    // Handles player death logic
    public void HandleDeath()
    {
        // Skip if player is already dead
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Disable player Movement
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Disable player controller
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Disables player input actions
        var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.Disable();
            playerInput.enabled = false;
        }

        // Display gameover text
        if (gameoverText != null)
        {
            gameoverText.gameObject.SetActive(true);
        }

        StartCoroutine(GoToMainMenuAfterDelay());
    }

    // Adds a delay after death before returning to main menu
    private IEnumerator GoToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(deathMessageDelay);

        SceneManagerScript.instance.LoadMainMenu();
    }
}
