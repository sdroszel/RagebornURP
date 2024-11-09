using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerJumpAndRoll : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private Animator animator;
    private PlayerControls playerControls;

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

    public bool IsJumping { get; private set; }
    public bool IsRolling { get; private set; }

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

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Jump.performed += OnJumpPerformed;
        playerControls.Player.Roll.performed += OnRollPerformed;
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Player.Jump.performed -= OnJumpPerformed;
            playerControls.Player.Roll.performed -= OnRollPerformed;
            playerControls.Disable();
        }
    }

    private void Update()
    {
        if (!playerController.playerStamina.CanConsumeStamina(rollStaminaCost))
        {
            rollCooldownImage.gameObject.SetActive(true);
        }
        else if (playerController.playerStamina.CanConsumeStamina(rollStaminaCost) && canRoll)
        {
            rollCooldownImage.gameObject.SetActive(false);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (playerController.groundCheck.IsGrounded() && canJump)
        {
            StartCoroutine(PerformJump());
        }
    }

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

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
        jumpCooldownImage.gameObject.SetActive(false);
    }

    private void OnRollPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.playerHealth.IsDead || PauseMenuScript.isGamePaused) return;

        if (playerController.groundCheck.IsGrounded() && canRoll && playerController.playerStamina.CanConsumeStamina(rollStaminaCost))
        {
            StartCoroutine(PerformRoll());
        }
    }

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

    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollTime);
        canRoll = true;
        rollCooldownImage.gameObject.SetActive(false);
    }
}
