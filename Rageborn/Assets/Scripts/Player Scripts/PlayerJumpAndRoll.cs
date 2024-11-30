using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerJumpAndRoll : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float forwardJumpForce = 4f;
    [SerializeField] private float jumpTime = 1f;
    [SerializeField] private float jumpCooldown = 1f;
    [Header("Roll Settings")]
    [SerializeField] private float rollSpeedMultiplier = 2f;
    [SerializeField] private float rollTime = 1f;
    [SerializeField] private float rollStaminaCost = 1f;
    [Header("UI Elements")]
    [SerializeField] private Image rollCooldownImage;
    [SerializeField] private TextMeshProUGUI rollCooldownText;
    [SerializeField] private Image jumpCooldownImage;

    private bool canJump = true;
    private bool canRoll = true;

    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private Animator animator;
    private PlayerControls playerControls;

    public bool IsJumping { get; private set; }
    public bool IsRolling { get; private set; }

    // Gets needed components and sets UI state
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        playerControls = new PlayerControls();

        jumpCooldownImage.gameObject.SetActive(false);
        rollCooldownImage.gameObject.SetActive(false);
    }

    // Enables player input
    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Jump.performed += OnJumpPerformed;
        playerControls.Player.Roll.performed += OnRollPerformed;
    }

    // Disables player input
    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Player.Jump.performed -= OnJumpPerformed;
            playerControls.Player.Roll.performed -= OnRollPerformed;
            playerControls.Disable();
        }
    }

    // Updates the UI elements every frame
    private void Update()
    {
        if (!playerController.playerStamina.CanConsumeStamina(rollStaminaCost))
        {
            // Show roll cooldown UI element if not enough stamina
            rollCooldownImage.gameObject.SetActive(true);
        }
        else if (playerController.playerStamina.CanConsumeStamina(rollStaminaCost) && canRoll)
        {
            // Hide rool cooldown UI if player has enough stamina and can roll
            rollCooldownImage.gameObject.SetActive(false);
        }
    }

    // Handles player input for jumping
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // Skip if player is dead or game is paused
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (playerController.groundCheck.IsGrounded() && canJump)
        {
            StartCoroutine(PerformJump());
        }
    }

    // Handles jump logic
    private IEnumerator PerformJump()
    {
        jumpCooldownImage.gameObject.SetActive(true);

        playerController.playerAudio.StopFootsteps();

        IsJumping = true;

        animator.SetBool("isJumping", true);

        rb.AddForce((Vector3.up * jumpForce) + (transform.forward * forwardJumpForce), ForceMode.Impulse);

        canJump = false;

        yield return new WaitForSeconds(jumpTime);

        animator.SetBool("isJumping", false);

        IsJumping = false;

        StartCoroutine(JumpCooldown());
    }

    // Handles jump cooldown
    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);

        canJump = true;

        jumpCooldownImage.gameObject.SetActive(false);
    }

    // Handles player roll input
    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        // Skip is player is dead or game is paused
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused || !playerMovement.IsMoving()) return;

        if (playerController.groundCheck.IsGrounded() && canRoll && playerController.playerStamina.CanConsumeStamina(rollStaminaCost))
        {
            // Perform roll if player is grounded, can roll, and has enough stamina
            StartCoroutine(PerformRoll());
        }
    }

    // Handles roll logic
    private IEnumerator PerformRoll()
    {
        rollCooldownImage.gameObject.SetActive(true);

        playerController.playerStamina.ConsumeStamina(rollStaminaCost);

        playerController.playerAudio.StopFootsteps();

        IsRolling = true;

        animator.SetBool("isRolling", true);

        float originalSpeed = playerMovement.CurrentMoveSpeed;

        playerMovement.CurrentMoveSpeed = originalSpeed * rollSpeedMultiplier;

        animator.speed = 2f;

        canRoll = false;

        yield return new WaitForSeconds(rollTime);

        playerMovement.CurrentMoveSpeed = originalSpeed;

        animator.SetBool("isRolling", false);

        animator.speed = 1f;

        IsRolling = false;

        StartCoroutine(RollCooldown());
        StartCoroutine(JumpCooldown());
    }

    // Handles roll cooldown
    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollTime);

        canRoll = true;

        rollCooldownImage.gameObject.SetActive(false);
    }
}
