using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the character
    public Transform playerCamera; // Reference to the player camera
    private Vector2 moveInput; // Store movement input
    private PlayerControls playerControls;
    private Rigidbody rb; // Reference to the Rigidbody
    private Animator animator; // Reference to the Animator

    private void Awake() 
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();

        // Bind move action to moveInput
        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void FixedUpdate() // Use FixedUpdate for physics calculations
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = playerCamera.TransformDirection(move); // Convert to camera-relative movement
        move.y = 0; // Prevent upward movement

        // Check if the character is moving
        bool isMoving = move != Vector3.zero;

        // Rotate the character to face the direction of movement
        if (isMoving) 
        {
            // Rotate the character to face the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }

        // Set running animation based on movement
        animator.SetBool("isRunning", isMoving); // Set isRunning to true or false based on movement

        // Move the character
        rb.MovePosition(rb.position + move.normalized * moveSpeed * Time.fixedDeltaTime); // Move using Rigidbody
    }

    private void OnDisable()
    {
        playerControls.Player.Disable();
    }
}
