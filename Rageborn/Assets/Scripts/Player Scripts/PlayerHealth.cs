using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float PotionHealAmount = 15f;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private ParticleSystem healEffect;
    [SerializeField] private AudioClip healAudio;
    [SerializeField] private AudioSource audioSource;
    [Header("Death Settings")]
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private float deathMessageDelay = 2f;


    private float currentHealth;
    private bool isDead = false;
    private UnityEngine.InputSystem.PlayerInput playerInput;
    private Animator animator;
    public bool IsDead => isDead;

    private void Awake()
    {
        healEffect.Stop();
        currentHealth = SceneManagerScript.instance.playerHealth;

        if (deathText != null)
        {
            deathText.gameObject.SetActive(false);
        }

        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        animator = GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.actions.Disable();
            playerInput.enabled = false;
        }
    }

    private void Update()
    {
        PotionHeal();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        Debug.Log("Taking damage");
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        SceneManagerScript.instance.playerHealth = currentHealth;

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;
        StartCoroutine(PerformHeal(healAmount));
    }

    private IEnumerator PerformHeal(float healAmount)
    {
        audioSource.PlayOneShot(healAudio);
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        SceneManagerScript.instance.playerHealth = currentHealth;
        healEffect.Play();

        yield return new WaitForSeconds(0.5f);

        healEffect.Stop();
    }

    private void PotionHeal()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (SceneManagerScript.instance.numOfHealthPotions > 0 && currentHealth != maxHealth)
            {
                Heal(PotionHealAmount);
                SceneManagerScript.instance.numOfHealthPotions -= 1;
            }
        }
    }

    public void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player has died.");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.Disable();
            playerInput.enabled = false;
        }

        if (deathText != null)
        {
            deathText.gameObject.SetActive(true);
        }

        StartCoroutine(GoToMainMenuAfterDelay());
    }

    private IEnumerator GoToMainMenuAfterDelay()
    {
        yield return new WaitForSeconds(deathMessageDelay);

        SceneManagerScript.instance.LoadMainMenu();
    }
}
