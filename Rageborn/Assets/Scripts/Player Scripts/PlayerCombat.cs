using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackTime = 1f;
    [SerializeField] private AudioClip attackSound;
    private PlayerController playerController;
    private Animator animator;
    private bool isAttacking = false;
    private int attackIndex = 0;
    private readonly string[] attacks = { "isAttacking1", "isAttacking2", "isAttacking3" };
    private PlayerControls playerControls;
    private AudioSource audioSource;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Attack.performed += Attack;
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Player.Attack.performed -= Attack;
            playerControls.Disable();
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        playerController.playerAudio.StopFootsteps();

        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        animator.SetBool(attacks[attackIndex], true);
        isAttacking = true;

        yield return new WaitForSeconds(attackTime);

        isAttacking = false;
        animator.SetBool(attacks[attackIndex], false);

        attackIndex = (attackIndex + 1) % attacks.Length;

        playerController.playerAudio.ResumeFootsteps();
    }

    public bool GetAttackStatus()
    {
        return isAttacking;
    }
}
