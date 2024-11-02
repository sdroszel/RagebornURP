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
    private PlayerControls playerControls;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        adjustedMoveSpeed = walkSpeed;

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();

        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Sprint.started += OnSprintStarted;
        playerControls.Player.Sprint.canceled += OnSprintCanceled;
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Player.Move.performed -= OnMovePerformed;
            playerControls.Player.Move.canceled -= OnMoveCanceled;
            playerControls.Player.Sprint.started -= OnSprintStarted;
            playerControls.Player.Sprint.canceled -= OnSprintCanceled;
            playerControls.Disable();
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (!playerController.groundCheck.IsGrounded())
        {
            return;
        }

        rb.AddForce(Vector3.down * 5f, ForceMode.Force);

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = playerCamera.TransformDirection(move);
        move.y = 0;

        bool isMoving = move != Vector3.zero;

        if (playerController.playerCombat.GetAttackStatus())
        {
            adjustedMoveSpeed = walkSpeed * 0.5f;
        }
        else if (isSprinting && playerController.playerStamina.CanSprint() && isMoving)
        {
            adjustedMoveSpeed = sprintSpeed;
            playerController.playerStamina.ConsumeSprint();
        }
        else
        {
            adjustedMoveSpeed = walkSpeed;
            playerController.playerStamina.ReplenishSprint();
        }

        if (isMoving && !playerController.playerJumpAndRoll.IsJumping && !playerController.playerJumpAndRoll.IsRolling)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            if (!playerController.playerAudio.AudioSource.isPlaying)
            {
                playerController.playerAudio.AudioSource.clip = playerController.playerAudio.GetCurrentRunningSound();
                playerController.playerAudio.AudioSource.loop = true;
                playerController.playerAudio.AudioSource.Play();
            }

            playerController.playerAudio.AudioSource.pitch = isSprinting ? 1.5f : 1.0f;
        }
        else
        {
            if (playerController.playerAudio.AudioSource.isPlaying)
            {
                playerController.playerAudio.AudioSource.Stop();
            }
        }

        animator.SetBool("isSprinting", isMoving && isSprinting && playerController.playerStamina.CanSprint());
        animator.SetBool("isRunning", isMoving);

        rb.MovePosition(rb.position + move.normalized * adjustedMoveSpeed * Time.fixedDeltaTime);
    }



    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnSprintStarted(InputAction.CallbackContext ctx)
    {
        if (playerController.playerStamina.CanSprint())
        {
            isSprinting = true;
        }
    }

    private void OnSprintCanceled(InputAction.CallbackContext ctx)
    {
        isSprinting = false;
    }

    public float CurrentMoveSpeed
    {
        get => adjustedMoveSpeed;
        set => adjustedMoveSpeed = value;
    }
}
