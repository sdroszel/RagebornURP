using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float isGroundedDelay = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.5f; // Adjust this based on character size
    [SerializeField] private float groundOffset = 0.1f; // Offset to position the check slightly above the player's feet
    private bool isGrounded;
    private float delayTimer;

    private void FixedUpdate()
    {
        CheckGroundStatus();

        // Update delay timer to smooth ground detection if needed
        if (isGrounded)
        {
            delayTimer = isGroundedDelay;
        }
        else
        {
            delayTimer -= Time.fixedDeltaTime;
        }
    }

    private void CheckGroundStatus()
    {
        // Use CheckSphere instead of SphereCast for simpler ground detection
        Vector3 checkPosition = transform.position + Vector3.up * groundOffset;
        isGrounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer);

        // Debugging output
        Debug.DrawRay(checkPosition, Vector3.down * (groundCheckRadius + groundOffset), Color.red);
        if (isGrounded)
        {
            Debug.Log("Ground detected");
        }
        else
        {
            Debug.Log("No ground detected");
        }
    }

    public bool IsGrounded()
    {
        return isGrounded || delayTimer > 0f;
    }
}
