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
        animator.SetBool("isJumping", true);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        canJump = false;

        yield return new WaitForSeconds(jumpTime);

        animator.SetBool("isJumping", false);

        StartCoroutine(JumpCooldown());
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
        animator.SetBool("isRolling", true);
        float originalSpeed = playerMovement.CurrentMoveSpeed;
        playerMovement.CurrentMoveSpeed = originalSpeed * rollSpeedMultiplier;
        animator.speed = 2f;
        canRoll = false;

        yield return new WaitForSeconds(rollTime);

        playerMovement.CurrentMoveSpeed = originalSpeed;
        animator.SetBool("isRolling", false);
        animator.speed = 1f;
        canRoll = true;

        StartCoroutine(RollCooldown());
        StartCoroutine(JumpCooldown());
    }

    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollTime);
    }
}
