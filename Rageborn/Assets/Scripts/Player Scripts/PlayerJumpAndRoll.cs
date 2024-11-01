using System;
using System.Collections;
using UnityEngine;

public class PlayerJumpAndRoll : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private Animator animator;

    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpTime = 1f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private float rollSpeedMultiplier = 2f; // Roll speed multiplier
    [SerializeField] private float rollTime = 1f;
    private bool canJump = true;
    private bool canRoll = true;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        var playerControls = new PlayerControls();
        playerControls.Player.Enable();
        playerControls.Player.Jump.performed += ctx => Jump();
        playerControls.Player.Roll.performed += ctx => Roll();
    }

    private void Jump() {
        if (playerController.groundCheck.IsGrounded() && canJump) {
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

    private IEnumerator JumpCooldown() {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
        animator.SetBool("isJumping", false); // Reset jump animation
    }

    private void Roll() {
        if (playerController.groundCheck.IsGrounded() && canRoll) {
            animator.SetBool("isRolling", true);
            canRoll = false;

            // Temporarily increase speed for the roll
            StartCoroutine(PerformRoll());
        }
    }

    private IEnumerator PerformRoll() {
        float originalSpeed = playerMovement.CurrentMoveSpeed;
        playerMovement.CurrentMoveSpeed = originalSpeed * rollSpeedMultiplier;

        animator.speed = 2f;

        yield return new WaitForSeconds(rollTime);

        // Reset roll state
        playerMovement.CurrentMoveSpeed = originalSpeed;
        animator.SetBool("isRolling", false);
        canRoll = true;
        canJump = false;
        animator.speed = 1f;

        StartCoroutine(RollCooldown());
        StartCoroutine(JumpCooldown());
    }

    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollTime);
    }
}
