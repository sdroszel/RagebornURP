using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] private Image sprintBarFill;
    [SerializeField] private float maxSprintDuration = 5f;
    [SerializeField] Transform playerCamera;
    [SerializeField] float attackTime = 1f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float jumpTime = 1f;
    [SerializeField] float rollTime = 1f;
    [SerializeField] AudioClip runningSnow;
    [SerializeField] AudioClip runningFloor;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpCooldown = 1f; // Cooldown duration for jumps
    private bool canJump = true;
    private float currentSprintDuration; // Current time remaining for sprinting

    private Vector2 moveInput;
    private PlayerControls playerControls;
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;
    private float adjustedMoveSpeed = 5f;
    private bool isAttacking = false;
    private bool isSprinting = false;
    private bool isDungeonFloor = false;
    private bool isGrounded = true;

    private void Awake() {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentSprintDuration = maxSprintDuration;
    }

    private void OnEnable() {
        playerControls.Player.Enable();

        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        playerControls.Player.Attack.performed += Attack;

        playerControls.Player.Sprint.started += ctx => StartSprinting();
        playerControls.Player.Sprint.canceled += ctx => StopSprinting();

        playerControls.Player.Jump.performed += ctx => Jump();
        //playerControls.Player.Roll.performed += ctx => Roll();
    }

    private void Update() {
        if (sprintBarFill != null) {
            float fillAmount = currentSprintDuration / maxSprintDuration; // Calculate fill amount
            sprintBarFill.fillAmount = fillAmount; // Update the fill amount
        }

    // Replenish the sprint gauge as before
        if (!isSprinting && currentSprintDuration < maxSprintDuration) {
            currentSprintDuration += Time.deltaTime; // Replenish sprint duration
            currentSprintDuration = Mathf.Min(currentSprintDuration, maxSprintDuration); // Clamp to max duration
        }
    }

    private void FixedUpdate() {
        MoveCharacter();
        CheckGroundStatus();
    }

    private void MoveCharacter() {
        if (isAttacking || !isGrounded) {
            return;
        }
        
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = playerCamera.TransformDirection(move);
        move.y = 0;

        bool isMoving = move != Vector3.zero;

        if (isSprinting && currentSprintDuration > 0) {
            adjustedMoveSpeed = sprintSpeed;
            if (isMoving) {
                currentSprintDuration -= Time.deltaTime; // Consume sprint duration
            }
        } else {
            adjustedMoveSpeed = walkSpeed;
            isSprinting = false; // Stop sprinting if the duration is exhausted
        }

        if (isMoving) {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            if (!audioSource.isPlaying) {
                audioSource.clip = isDungeonFloor ? runningFloor : runningSnow;
                audioSource.loop = true;
                audioSource.Play();
            }
        } else {
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
        }

        animator.SetBool("isSprinting", isMoving && isSprinting);
        animator.SetBool("isRunning", isMoving);

        rb.MovePosition(rb.position + move.normalized * adjustedMoveSpeed * Time.fixedDeltaTime);
    }

    private void Attack(InputAction.CallbackContext context) {
        if (context.performed && !isAttacking) {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack() {
        int randNum = Random.Range(0, 3);

        if (randNum == 0) {
            animator.SetBool("isAttacking1", true);
        }
        else if (randNum == 1) {
            animator.SetBool("isAttacking2", true);
        }
        else {
            animator.SetBool("isAttacking3", true);
        }
        
        isAttacking = true;

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        
        if (randNum == 0) {
            animator.SetBool("isAttacking1", false);
        }
        else if (randNum == 1) {
            animator.SetBool("isAttacking2", false);
        }
        else {
            animator.SetBool("isAttacking3", false);
        }
    }

    private void StartSprinting() {
        if (currentSprintDuration > 0) {
            isSprinting = true;
        }
    }

    private void StopSprinting() {
        isSprinting = false;
    }

    private void Jump() {
        if (isGrounded && canJump && !isAttacking) {
            animator.SetBool("isJumping", true);

            // Apply force for the jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isGrounded = false; // The player is not grounded anymore
            canJump = false; // Disable jumping

            // Start the cooldown coroutine
            StartCoroutine(JumpCooldown());
        }
    }

/*     private void Roll() {
        if (isGrounded && !isAttacking) {
            animator.speed = 2f;
            animator.SetBool("isRolling", true);

            StartCoroutine(RollCooldown());
        }
    }

    private IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollTime);

        animator.SetBool("isRolling", false);
        animator.speed = 1f;
    } */

    private IEnumerator JumpCooldown() {
        yield return new WaitForSeconds(jumpCooldown); // Wait for the cooldown duration
        canJump = true; // Re-enable jumping
    }

    private void CheckGroundStatus() {
        float raycastDistance = 1.1f; // Adjust based on player height
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Start raycast slightly above player

        bool wasGrounded = isGrounded; // Store previous grounded state
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, groundLayer);

        if (isGrounded && !wasGrounded) {
            // The player has just landed
            StartCoroutine(HandleLanding());
        }

        // Optional debug line to visualize the raycast in the scene view
        Debug.DrawRay(rayOrigin, Vector3.down * raycastDistance, isGrounded ? Color.green : Color.red);
    }

    private IEnumerator HandleLanding() {
        // Wait for a short duration to allow the landing animation to play
        yield return new WaitForSeconds(jumpTime); // Adjust this value based on your animation duration

        // Reset the jump state and play landing animation if needed
        animator.SetBool("isJumping", false);
    }

    private void OnDisable() {
        playerControls.Player.Disable();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Floor")) {
            isDungeonFloor = true;
            audioSource.Stop();
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Floor")) {
            isDungeonFloor = false;
            audioSource.Stop();
        }
    }
}
