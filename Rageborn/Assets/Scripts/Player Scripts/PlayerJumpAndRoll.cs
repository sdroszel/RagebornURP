using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpAndRoll : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private Animator animator;
    private PlayerControls playerControls;

    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpTime = 1f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float rollSpeedMultiplier = 2f; // Roll speed multiplier
    [SerializeField] private float rollTime = 1f;
    private bool canJump = true;
    private bool canRoll = true;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Initialize PlayerControls only once in Awake
        playerControls = new PlayerControls();
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

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.groundCheck.IsGrounded() && canJump)
        {
            StartCoroutine(PerformJump());
        }
    }

    private IEnumerator PerformJump()
    {
        playerController.playerAudio.StopFootsteps(); // Stop footsteps audio at start of jump
        animator.SetBool("isJumping", true);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;

        yield return new WaitForSeconds(jumpTime); // Wait for jump animation to complete

        animator.SetBool("isJumping", false);
        yield return StartCoroutine(JumpCooldown()); // Wait for cooldown before allowing next jump

        playerController.playerAudio.ResumeFootsteps(); // Resume footsteps only after action and cooldown are complete
    }

    private IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void OnRollPerformed(InputAction.CallbackContext ctx)
    {
        if (playerController.groundCheck.IsGrounded() && canRoll)
        {
            StartCoroutine(PerformRoll());
        }
    }

    private IEnumerator PerformRoll()
    {
        playerController.playerAudio.StopFootsteps(); // Stop footsteps audio at start of roll

        animator.SetBool("isRolling", true);
        float originalSpeed = playerMovement.CurrentMoveSpeed;
        playerMovement.CurrentMoveSpeed = originalSpeed * rollSpeedMultiplier;
        animator.speed = 2f;
        canRoll = false;

        yield return new WaitForSeconds(rollTime); // Wait for roll animation to complete

        playerMovement.CurrentMoveSpeed = originalSpeed;
        animator.SetBool("isRolling", false);
        animator.speed = 1f;

        yield return StartCoroutine(RollCooldown()); // Wait for cooldown before allowing next roll

        playerController.playerAudio.ResumeFootsteps(); // Resume footsteps only after action and cooldown are complete
    }

    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollTime);
        canRoll = true;
    }
}
