using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private float deathMessageDelay = 2f;


    private float currentHealth;
    private bool isDead = false;
    private UnityEngine.InputSystem.PlayerInput playerInput;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (deathText != null)
        {
            deathText.gameObject.SetActive(false);
        }

        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
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

        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void HandleDeath()
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
