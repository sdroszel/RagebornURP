using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player character to follow
    public float distance = 5f; // Distance from the target
    public float sensitivity = 2f; // Mouse sensitivity
    public float minYAngle = -40f; // Minimum vertical angle
    public float maxYAngle = 80f; // Maximum vertical angle

    private float currentX = 0f; // Horizontal rotation
    private float currentY = 0f; // Vertical rotation
    private Vector2 lookInput; // Store mouse input
    private PlayerControls inputActions; // Input actions
    private bool isLooking = false; // Flag to check if right mouse button is held down

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        // Bind look action to lookInput (Mouse/Delta)
        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        // Bind right mouse button actions to check if it's held down
        inputActions.Player.MouseLook.started += ctx => isLooking = true;
        inputActions.Player.MouseLook.canceled += ctx => isLooking = false;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Follow the player's position
            Vector3 targetPosition = target.position;

            // Only allow camera rotation if the right mouse button is held down
            if (isLooking)
            {
                currentX += lookInput.x * sensitivity;
                currentY -= lookInput.y * sensitivity;
                currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
            }

            // Calculate rotation and position
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 position = targetPosition - (rotation * Vector3.forward * distance);

            // Set camera position and rotation
            transform.position = position;
            transform.LookAt(targetPosition);
        }
    }

    private void OnDisable()
    {
        // Detach input callbacks to avoid memory leaks
        inputActions.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled -= ctx => lookInput = Vector2.zero;
        inputActions.Player.MouseLook.started -= ctx => isLooking = true;
        inputActions.Player.MouseLook.canceled -= ctx => isLooking = false;

        inputActions.Player.Disable();
    }
}
