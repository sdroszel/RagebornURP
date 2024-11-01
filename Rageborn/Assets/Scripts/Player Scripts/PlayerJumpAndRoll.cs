using System.Collections;
using UnityEngine;

public class PlayerJumpAndRoll : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody rb;
    private Animator animator;

    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpCooldown = 1f;
    private bool canJump = true;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Jump() {
        if (playerController.groundCheck.IsGrounded() && canJump) {
            animator.SetBool("isJumping", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canJump = false;
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown() {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}
