using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float distance = 5f;
    [SerializeField] float sensitivity = 2f;
    [SerializeField] float minYAngle = -40f;
    [SerializeField] float maxYAngle = 80f;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float startYAngle = 20f;
    [SerializeField] float cameraRadius = 0.5f;
    [SerializeField] float minDistance = 1f; // Minimum distance to prevent getting too close
    [SerializeField] float groundOffset = 0.5f; // Distance above terrain to prevent clipping

    private float currentX = 0f;
    private float currentY;
    private Vector2 lookInput;
    private PlayerControls inputActions;
    private bool isLooking = false;

    private void Awake() {
        inputActions = new PlayerControls();
        currentY = startYAngle;
    }
    
    private void OnEnable() {
        inputActions.Player.Enable();

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.MouseLook.started += ctx => isLooking = true;
        inputActions.Player.MouseLook.canceled += ctx => isLooking = false;
    }

    private void LateUpdate() {
        if (target != null) {
            Vector3 targetPosition = target.position;

            // Update camera rotation based on look input
            currentX += lookInput.x * sensitivity;
            currentY -= lookInput.y * sensitivity;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

            // SphereCast to check for obstacles between target and desired camera position
            RaycastHit hit;
            Vector3 direction = (desiredPosition - targetPosition).normalized;
            float rayDistance = (desiredPosition - targetPosition).magnitude;

            if (Physics.SphereCast(targetPosition, cameraRadius, direction, out hit, rayDistance, obstacleMask)) {
                // Adjust the camera distance if there is an obstacle
                float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, distance);
                desiredPosition = targetPosition - (rotation * Vector3.forward * adjustedDistance);

                // Stop at minDistance if too close
                if (adjustedDistance <= minDistance + 0.1f) {
                    desiredPosition = targetPosition - (rotation * Vector3.forward * minDistance);
                }
            }

            // Downward raycast to keep camera above ground
            if (Physics.Raycast(desiredPosition, Vector3.down, out hit, Mathf.Infinity, obstacleMask)) {
                // Ensure the camera stays above the terrain height
                desiredPosition.y = Mathf.Max(desiredPosition.y, hit.point.y + groundOffset);
            }

            // Set the camera position and rotation
            transform.position = desiredPosition;
            transform.LookAt(targetPosition);
        }
    }

    private void OnDisable() {
        inputActions.Player.Disable();
    }
}
