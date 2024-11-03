using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerController playerController;
    private Rigidbody rb;
    private Animator animator;
    private bool isAttacking = false;
    private bool isSpinAttacking = false;
    private bool isSpinAttackOnCooldown = false;
    private int attackIndex = 0;
    private readonly string[] attacks = { "isAttacking1", "isAttacking2", "isAttacking3" };
    private PlayerControls playerControls;
    private int currentAttackDamage;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        playerControls = new PlayerControls();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        DisableWeaponCollider();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Attack.performed += Attack;
        playerControls.Player.SpinAttack.performed += SpinAttack;
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Player.Attack.performed -= Attack;
            playerControls.Player.SpinAttack.performed -= SpinAttack;
            playerControls.Disable();
        }
    }

    private void SpinAttack(InputAction.CallbackContext context)
    {
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (!isSpinAttackOnCooldown && playerController.playerStamina.CanConsumeStamina(spinAttackStaminaCost))
        {
            currentAttackDamage = spinAttackDamage;
            StartCoroutine(PerformSpinAttack());
        }
    }

    private IEnumerator PerformSpinAttack()
    {
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

        yield return new WaitForSeconds(spinAttackCooldown);
        isSpinAttackOnCooldown = false;
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (context.performed && !isAttacking)
        {
            currentAttackDamage = normalAttackDamage;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        playerController.playerAudio.StopFootsteps();

        footstepAudioSource.pitch = 0.5f;

        if (attackSound != null)
        {
            attackAudioSource.PlayOneShot(attackSound);
        }

        animator.SetBool(attacks[attackIndex], true);
        isAttacking = true;

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        animator.SetBool(attacks[attackIndex], false);

        attackIndex = (attackIndex + 1) % attacks.Length;

        playerController.playerAudio.ResumeFootsteps();
    }

    private void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    private void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && weaponCollider.enabled)
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(currentAttackDamage);
                attackAudioSource.PlayOneShot(hitSound);
            }
        }
    }

    public bool GetAttackStatus()
    {
        return isAttacking;
    }

    public bool GetSpinAttackStatus()
    {
        return isSpinAttacking;
    }
}
