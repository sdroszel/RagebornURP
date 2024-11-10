using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Normal Attack Settings")]
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private int normalAttackDamage = 20;
    [SerializeField] private AudioClip attackSound;
    
    [Header("Spin Attack Settings")]
    [SerializeField] private float SpinAttackTime = 1f;
    [SerializeField] private float spinAttackCooldown = 5f;
    [SerializeField] private float spinAttackStaminaCost = 1f;
    [SerializeField] private int spinAttackDamage = 30;
    [SerializeField] private float spinAttackBurstSpeed = 5f;
    [SerializeField] private AudioClip spinAttackSound;
    
    [Header("Weapon Collider")]
    [SerializeField] private Collider weaponCollider;
    [SerializeField] private AudioClip hitSound;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioSource footstepAudioSource;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI spinAttackCooldownText;
    [SerializeField] private Image spinAttackCooldownImage;
    [SerializeField] private Image basicAttackCooldownImage;

    private bool isAttacking = false;
    private bool isSpinAttacking = false;
    private bool isSpinAttackOnCooldown = false;

    private int attackIndex = 0;
    private int currentAttackDamage;

    private readonly string[] attacks = { "isAttacking1", "isAttacking2", "isAttacking3" };

    private PlayerControls playerControls;
    private PlayerController playerController;
    private Animator animator;

    // Initializes references, sets up default UI states, and disables the weapon collider
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        playerControls = new PlayerControls();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        DisableWeaponCollider();

        spinAttackCooldownText.text = "";
        spinAttackCooldownImage.gameObject.SetActive(false);
        basicAttackCooldownImage.gameObject.SetActive(false);
    }

    // Enables player input controls and binds attack actions to input events
    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Attack.performed += Attack;
        playerControls.Player.SpinAttack.performed += SpinAttack;
    }

    // Disables player input controls and unbinds attack actions to avoid memory leaks
    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Player.Attack.performed -= Attack;
            playerControls.Player.SpinAttack.performed -= SpinAttack;
            playerControls.Disable();
        }
    }

    // Updates the spin attack UI element based on the player's stamina and cooldown status
    private void Update()
    {
        if (!playerController.playerStamina.CanConsumeStamina(spinAttackStaminaCost))
        {
            spinAttackCooldownImage.gameObject.SetActive(true);
        }
        else if (playerController.playerStamina.CanConsumeStamina(spinAttackStaminaCost) && !isSpinAttackOnCooldown)
        {
            spinAttackCooldownImage.gameObject.SetActive(false);
        }
    }

    // Handles the spin attack input, starting the spin attack if conditions are met
    private void SpinAttack(InputAction.CallbackContext context)
    {
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (!isSpinAttackOnCooldown && playerController.playerStamina.CanConsumeStamina(spinAttackStaminaCost))
        {
            currentAttackDamage = spinAttackDamage;
            StartCoroutine(PerformSpinAttack());
        }
    }

    // Performs the spin attack, applies cooldown, and manages related animations and audio
    private IEnumerator PerformSpinAttack()
    {
        spinAttackCooldownImage.gameObject.SetActive(true);

        playerController.playerStamina.ConsumeStamina(spinAttackStaminaCost);

        playerController.playerAudio.StopFootsteps();

        footstepAudioSource.pitch = 0.5f;

        if (spinAttackSound != null)
        {
            attackAudioSource.PlayOneShot(spinAttackSound);
        }

        animator.SetBool("isSpinAttack", true);

        isSpinAttacking = true;
        isSpinAttackOnCooldown = true;

        if (playerController.playerMovement.IsMoving())
        {
            Vector3 burstDirection = transform.forward * spinAttackBurstSpeed * Time.deltaTime;
            transform.position += burstDirection;
        }

        yield return new WaitForSeconds(SpinAttackTime);

        isSpinAttacking = false;

        animator.SetBool("isSpinAttack", false);

        playerController.playerAudio.ResumeFootsteps();

        StartCoroutine(SpinAttackCooldownCountdown());
    }

    // Handles the countdown for the spin attack's cooldown and updates the UI
    private IEnumerator SpinAttackCooldownCountdown()
    {
        float elapsedTime = 0f;
        while (elapsedTime < spinAttackCooldown)
        {
            elapsedTime += Time.deltaTime;

            float remainingTime = spinAttackCooldown - elapsedTime;
            spinAttackCooldownText.text = Mathf.Ceil(remainingTime).ToString();

            yield return null;
        }

        spinAttackCooldownText.text = "";
        spinAttackCooldownImage.gameObject.SetActive(false);
        isSpinAttackOnCooldown = false;
    }

    // Handles the normal attack input, starting the attack if conditions are met
    private void Attack(InputAction.CallbackContext context)
    {
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (context.performed && !isAttacking)
        {
            currentAttackDamage = normalAttackDamage;
            StartCoroutine(PerformAttack());
        }
    }

    // Executes the normal attack, plays the attack animation and sound, and manages cooldowns
    private IEnumerator PerformAttack()
    {
        basicAttackCooldownImage.gameObject.SetActive(true);
        playerController.playerAudio.StopFootsteps();

        footstepAudioSource.pitch = 0.5f;

        if (attackSound != null)
        {
            attackAudioSource.PlayOneShot(attackSound);
        }

        animator.SetBool(attacks[attackIndex], true);
        isAttacking = true;

        yield return new WaitForSeconds(attackTime);
        basicAttackCooldownImage.gameObject.SetActive(false);
        isAttacking = false;
        animator.SetBool(attacks[attackIndex], false);

        attackIndex = (attackIndex + 1) % attacks.Length;

        playerController.playerAudio.ResumeFootsteps();
    }

    // Enables the weapon collider for detecting hits on enemies during an attack
    private void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    // Disables the weapon collider to prevent unintended hits outside of an attack
    private void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    // Detects collisions with enemies when the weapon collider is active and applies damage
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Enemy") || other.CompareTag("RangedEnemy")) && weaponCollider.enabled)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyController enemy = other.GetComponent<EnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(currentAttackDamage);
                    attackAudioSource.PlayOneShot(hitSound);
                }
            }
            else
            {
                CasterEnemyController enemy = other.GetComponent<CasterEnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(currentAttackDamage);
                    attackAudioSource.PlayOneShot(hitSound);
                }
            }
        }
    }

    // Returns whether a normal attack is currently active
    public bool GetAttackStatus()
    {
        return isAttacking;
    }

    // Returns whether a spin attack is currently active
    public bool GetSpinAttackStatus()
    {
        return isSpinAttacking;
    }
}
