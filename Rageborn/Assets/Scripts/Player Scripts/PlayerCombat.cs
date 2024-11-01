using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackTime = 1f;
    private PlayerController playerController;
    private Animator animator;
    private bool isAttacking = false;
    private int attackIndex = 0;
    private readonly string[] attacks = { "isAttacking1", "isAttacking2", "isAttacking3" };
    private PlayerControls playerControls;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        // Initialize PlayerControls only once in Awake
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.Attack.performed += Attack;
    }

    private void OnDisable()
    {
        // Unsubscribe from the input event when disabled to prevent lingering references
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
        animator.SetBool(attacks[attackIndex], true);
        isAttacking = true;

        yield return new WaitForSeconds(attackTime); // Adjust this for the timing of your attack

        isAttacking = false;
        animator.SetBool(attacks[attackIndex], false);

        attackIndex = (attackIndex + 1) % attacks.Length;
    }

    public bool GetAttackStatus()
    {
        return isAttacking;
    }
}
