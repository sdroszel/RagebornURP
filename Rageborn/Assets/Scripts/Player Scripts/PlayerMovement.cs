using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody rb;
    private Animator animator;
    private Vector2 moveInput;
    private float adjustedMoveSpeed;

    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private Transform playerCamera;
    private bool isSprinting;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        adjustedMoveSpeed = walkSpeed;
    }

    private void OnEnable() {
        var playerControls = new PlayerControls();
        playerControls.Player.Enable();
        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        playerControls.Player.Sprint.started += ctx => StartSprinting();
        playerControls.Player.Sprint.canceled += ctx => StopSprinting();
    }

    private void FixedUpdate() {
        MoveCharacter();
    }

    private void MoveCharacter() {
        if (playerController.playerCombat.GetAttackStatus() || !playerController.groundCheck.IsGrounded()) {
            return;
        }

        rb.AddForce(Vector3.down * 5f, ForceMode.Force);

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = playerCamera.TransformDirection(move);
        move.y = 0;

        bool isMoving = move != Vector3.zero;

        // Sprint if the button is held, stamina is available, and the player is moving
        if (isSprinting && playerController.playerStamina.CanSprint() && isMoving) {
            adjustedMoveSpeed = sprintSpeed;
            playerController.playerStamina.ConsumeSprint(); // Consume stamina when sprinting
        } else {
            adjustedMoveSpeed = walkSpeed;
            playerController.playerStamina.ReplenishSprint(); // Replenish stamina when not sprinting
        }

        if (isMoving) {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // Manage footsteps audio
            if (!playerController.playerAudio.AudioSource.isPlaying) {
                playerController.playerAudio.AudioSource.clip = playerController.playerAudio.GetCurrentRunningSound();
                playerController.playerAudio.AudioSource.loop = true;
                playerController.playerAudio.AudioSource.Play();
            }
        } else {
            if (playerController.playerAudio.AudioSource.isPlaying) {
                playerController.playerAudio.AudioSource.Stop();
            }
        }

        animator.SetBool("isSprinting", isMoving && isSprinting && playerController.playerStamina.CanSprint());
        animator.SetBool("isRunning", isMoving);

        rb.MovePosition(rb.position + move.normalized * adjustedMoveSpeed * Time.fixedDeltaTime);
    }

    private void StartSprinting() {
        if (playerController.playerStamina.CanSprint()) {
            isSprinting = true;
        }
    }

    private void StopSprinting() {
        isSprinting = false;
    }

    public float CurrentMoveSpeed {
        get => adjustedMoveSpeed;
        set => adjustedMoveSpeed = value;
    }
}
