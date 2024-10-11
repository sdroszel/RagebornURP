using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform playerCamera;
    [SerializeField] float attackTime = 1f;
    [SerializeField] AudioClip runningSnow;
    [SerializeField] AudioClip runningFloor;
    private Vector2 moveInput;
    private PlayerControls playerControls;
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;
    private float adjustedMoveSpeed = 5f;
    private bool isAttacking = false;
    private bool isDungeonFloor = false;

    private void Awake() {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

            if (!audioSource.isPlaying) {
                if (!isDungeonFloor) {
                    audioSource.clip = runningSnow;
                }
                else {
                    audioSource.clip = runningFloor;
                }
                audioSource.loop = true;
                audioSource.Play();
            }
        } else {
            
            if (audioSource.isPlaying) {
                audioSource.Stop();
            }
        }

        animator.SetBool("isRunning", isMoving);

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

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Floor")) {
            isDungeonFloor = true;
            audioSource.Stop();
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.CompareTag("Floor")) {
            isDungeonFloor = false;
            audioSource.Stop();
        }
    }
}
