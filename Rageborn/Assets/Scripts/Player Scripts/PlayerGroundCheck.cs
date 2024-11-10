using UnityEngine;

/// <summary>
/// This class handles the player groundcheck for movement, rolling, and jumping
/// </summary>
public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float isGroundedDelay = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.5f;
    [SerializeField] private float groundOffset = 0.1f;

    private bool isGrounded;
    private float delayTimer;

    private void FixedUpdate()
    {
        CheckGroundStatus();

        // Uses coyote time to delay groundcheck for stairs and ledges
        if (isGrounded)
        {
            delayTimer = isGroundedDelay;
        }
        else
        {
            delayTimer -= Time.fixedDeltaTime;
        }
    }

    // Checks for collision between player and ground layer
    private void CheckGroundStatus()
    {
        // Use CheckSphere for ground detection
        Vector3 checkPosition = transform.position + Vector3.up * groundOffset;
        isGrounded = Physics.CheckSphere(checkPosition, groundCheckRadius, groundLayer);
    }

    // Used to see if player is grounded
    public bool IsGrounded()
    {
        return isGrounded || delayTimer > 0f;
    }
}
