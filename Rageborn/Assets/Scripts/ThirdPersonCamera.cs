using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player character to follow
    public float distance = 5f; // Distance from the target
    public float sensitivity = 2f; // Mouse sensitivity
    public float minYAngle = -40f; // Minimum vertical angle
    public float maxYAngle = 80f; // Maximum vertical angle
    public LayerMask obstacleMask; // LayerMask to specify obstacle layers

    private float currentX = 0f; // Horizontal rotation
    private float currentY = 0f; // Vertical rotation
    private Vector2 lookInput; // Store mouse input
    private PlayerControls inputActions;
    private bool isLooking = false; // Flag to check if right mouse button is held down

    private void Awake() {
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

        // Calculate desired rotation and position
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

        // Raycast to check for obstacles
        RaycastHit hit;
        Vector3 direction = (desiredPosition - targetPosition).normalized; // Direction to camera
        float rayDistance = (desiredPosition - targetPosition).magnitude; // Distance to check

        if (Physics.Raycast(targetPosition, direction, out hit, rayDistance, obstacleMask))
        {
            // If there is an obstacle, position the camera at the hit point minus a small offset
            transform.position = hit.point + hit.normal * 0.5f; // Offset slightly away from the wall
        }
        else
        {
            // Set camera position to the desired position
            transform.position = desiredPosition;
        }

        // Ensure camera rotation
        transform.LookAt(targetPosition);
    }
}



    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
