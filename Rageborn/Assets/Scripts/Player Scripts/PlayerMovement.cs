using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// This class handles player movement
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    
    [Header("Reference to Camera")]
    [SerializeField] private Transform playerCamera;
    private bool isSprinting;
    private PlayerControls playerControls;

    [Header("UI Elements")]
    [SerializeField] private Image cooldownBG;

    private PlayerController playerController;
    private Rigidbody rb;
    private Animator animator;
    private PlayerCombat playerCombat;
    private Vector2 moveInput;

    private float adjustedMoveSpeed;
    private PlayerRage playerRage;
    // Gets needed components and sets default movement speed
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        adjustedMoveSpeed = walkSpeed;

        playerControls = new PlayerControls();
        playerRage = GetComponent<PlayerRage>();
        playerCombat = GetComponent<PlayerCombat>(); // Get PlayerCombat component
    }

    // Enables player input controls
    private void OnEnable()
    {
        playerControls.Player.Enable();

        playerControls.Player.Move.performed += OnMovePerformed;
        playerControls.Player.Move.canceled += OnMoveCanceled;
        playerControls.Player.Sprint.started += OnSprintStarted;
        playerControls.Player.Sprint.canceled += OnSprintCanceled;
    }

    // Disables player input controls
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

    // Updates every frame to update UI elements
    private void Update()
    {
        if (playerController.playerStamina.CanSprint())
        {
            // Hide sprint cooldown UI if player has enough stamina
            cooldownBG.gameObject.SetActive(false);
        }
        else
        {
            // Shows sprint cooldown UI if player is out of stamina
            cooldownBG.gameObject.SetActive(true);
        }

       // Check if the R key is pressed and rage is full
        if (playerRage.IsRageActive())
        {
            Debug.Log("Rage is active!");
        }
        if (Input.GetKeyDown(KeyCode.R) && playerRage.IsRageFull())
        {
        playerRage.ActivateRage();
        }

        if (Input.GetKeyDown(KeyCode.R) && playerRage.IsRageActive())
        {
        playerRage.DeactivateRage();
        }
    }

    // Update for character movement
    private void FixedUpdate()
    {
        MoveCharacter();
    }

    // Handles character movement
    private void MoveCharacter()
    {
        if (PauseMenuScript.isGamePaused)
        {
            StopFootstepAudio();
            return;
        }

        // Skip if player is not grounded
        if (!playerController.groundCheck.IsGrounded())
        {
            return;
        }

        rb.AddForce(Vector3.down * 5f, ForceMode.Force);

        // Gets player movement vector
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = playerCamera.TransformDirection(move);
        move.y = 0;

        bool isMoving = move != Vector3.zero;

        AdjustMovementSpeed(isMoving);

        if (isMoving && !playerController.playerJumpAndRoll.IsJumping && !playerController.playerJumpAndRoll.IsRolling)
        {
            RotateCharacterTowardsMovement(move);
            UpdateFootstepAudio();
        }
        else
        {
            StopFootstepAudio();
        }

        animator.SetBool("isSprinting", isMoving && isSprinting && playerController.playerStamina.CanSprint());
        animator.SetBool("isRunning", isMoving);

        rb.MovePosition(rb.position + move.normalized * adjustedMoveSpeed * Time.fixedDeltaTime);
    }

    // Helper function to adjust player movement speed based on player actions
    private void AdjustMovementSpeed(bool isMoving)
    {
        if (playerController.playerCombat.GetAttackStatus())
        {
            adjustedMoveSpeed = walkSpeed * 0.5f;
            playerController.playerAudio.AudioSource.pitch = 0.5f;
        }
        else if (playerController.playerCombat.GetSpinAttackStatus())
        {
            adjustedMoveSpeed = walkSpeed * 2f;
        }
        else if (isSprinting && playerController.playerStamina.CanSprint() && isMoving)
        {
            adjustedMoveSpeed = sprintSpeed;
            playerController.playerAudio.AudioSource.pitch = 1.5f;
            playerController.playerStamina.ConsumeSprint();
        }
        else
        {
            adjustedMoveSpeed = walkSpeed;
            playerController.playerAudio.AudioSource.pitch = 1.2f;
            playerController.playerStamina.ReplenishStamina();
        }
    }

    // Rotates player to face the movement direction
    private void RotateCharacterTowardsMovement(Vector3 move)
    {
        Quaternion targetRotation = Quaternion.LookRotation(move);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    // Updates the player footstep audio
    private void UpdateFootstepAudio()
    {
        if (!playerController.playerAudio.AudioSource.isPlaying)
        {
            playerController.playerAudio.AudioSource.clip = playerController.playerAudio.GetCurrentRunningSound();
            playerController.playerAudio.AudioSource.loop = true;
            playerController.playerAudio.AudioSource.Play();
        }
    }

    // Stops the player footstep audio
    private void StopFootstepAudio()
    {
        if (playerController.playerAudio.AudioSource.isPlaying)
        {
            playerController.playerAudio.AudioSource.Stop();
        }
    }

    // Handles player move input
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Handles stoping player move input
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    // Handles player sprint input
    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        if (playerController.playerStamina.CanSprint())
        {
            isSprinting = true;
        }
    }

    // Handles player stop sprinting input
    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    // Public getter/setter for movement speed
    public float CurrentMoveSpeed
    {
        get => adjustedMoveSpeed;
        set => adjustedMoveSpeed = value;
    }

    // Used to see if player is moving
    public bool IsMoving()
    {
        return moveInput != Vector2.zero;
    }
}
