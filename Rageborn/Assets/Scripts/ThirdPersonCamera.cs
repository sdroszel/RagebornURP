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
    [SerializeField] float groundOffset = 0.5f;
    [SerializeField] float minDistance = 1f;

    private float currentX = 0f;
    private float currentY;
    private Vector2 lookInput;
    private PlayerControls inputActions;
    private bool isLooking = false;

    private void Awake()
    {
        inputActions = new PlayerControls();
        currentY = startYAngle;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position;

            currentX += lookInput.x * sensitivity;
            currentY -= lookInput.y * sensitivity;
            currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);

            RaycastHit hit;
            Vector3 direction = (desiredPosition - targetPosition).normalized;
            float rayDistance = Vector3.Distance(targetPosition, desiredPosition);

            if (Physics.Raycast(targetPosition, direction, out hit, rayDistance, obstacleMask))
            {
                float adjustedDistance = Mathf.Clamp(hit.distance, minDistance, distance);
                desiredPosition = targetPosition - (rotation * Vector3.forward * adjustedDistance);
            }

            if (Physics.Raycast(desiredPosition, Vector3.down, out hit, Mathf.Infinity, obstacleMask))
            {
                desiredPosition.y = Mathf.Max(desiredPosition.y, hit.point.y + groundOffset);
            }

            transform.position = desiredPosition;
            transform.LookAt(targetPosition);
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
}
