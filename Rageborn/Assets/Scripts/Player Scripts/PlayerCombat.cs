using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    private bool isAttacking = false;
    private int attackIndex = 0;
    private readonly string[] attacks = { "isAttacking1", "isAttacking2", "isAttacking3" };

    private void Awake() {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        var playerControls = new PlayerControls();
        playerControls.Player.Enable();
        playerControls.Player.Attack.performed += ctx => Attack(ctx);
    }

    private void Attack(InputAction.CallbackContext context) {
        if (context.performed && !isAttacking) {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack() {
        animator.SetBool(attacks[attackIndex], true);
        isAttacking = true;

        yield return new WaitForSeconds(1f); // Adjust this for attack timing

        isAttacking = false;
        animator.SetBool(attacks[attackIndex], false);

        attackIndex = (attackIndex + 1) % attacks.Length;
    }

    public bool GetAttackStatus() {
        return isAttacking;
    }
}
