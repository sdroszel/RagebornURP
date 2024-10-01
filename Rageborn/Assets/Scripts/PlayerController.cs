using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform playerCamera;
    [SerializeField] float attackTime = 1f;
    private Vector2 moveInput;
    private PlayerControls playerControls;
    private Rigidbody rb;
    private Animator animator;
    private float adjustedMoveSpeed = 5f;
    private bool isAttacking = false;

    private void Awake() {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        playerControls.Player.Enable();

        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        playerControls.Player.Attack.performed += Attack;
        
    }


    private void FixedUpdate() {
        MoveCharacter();
    }

    private void MoveCharacter() {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        move = playerCamera.TransformDirection(move);
        move.y = 0;

        bool isMoving = move != Vector3.zero;

        if (isMoving) {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        animator.SetBool("isRunning", isMoving);

        // Move the character
        rb.MovePosition(rb.position + move.normalized * adjustedMoveSpeed * Time.fixedDeltaTime);
    }

    private void Attack(InputAction.CallbackContext context) {
        if (context.performed && !isAttacking) {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack() {
        animator.SetBool("isAttacking", true);
        isAttacking = true;

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    private void OnDisable() {
        playerControls.Player.Disable();
    }
}
