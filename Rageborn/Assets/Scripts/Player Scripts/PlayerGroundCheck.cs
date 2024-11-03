using UnityEngine;
using UnityEngine.Events;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float isGroundedDelay = 0.2f;
    public UnityEvent OnLand;
    private bool isGrounded;
    private float delayTimer;

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        CheckGroundStatus();

        if (!wasGrounded && isGrounded)
        {
            OnLand?.Invoke();
        }

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
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, 0.6f, groundLayer);
    }

    public bool IsGrounded()
    {
        return isGrounded || delayTimer > 0f;
    }
}
