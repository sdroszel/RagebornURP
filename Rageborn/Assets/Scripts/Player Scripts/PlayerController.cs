using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerCombat playerCombat;
    [HideInInspector] public PlayerJumpAndRoll playerJumpAndRoll;
    [HideInInspector] public PlayerGroundCheck groundCheck;
    [HideInInspector] public PlayerStamina playerStamina;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public PlayerAudio playerAudio;

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerJumpAndRoll = GetComponent<PlayerJumpAndRoll>();
        groundCheck = GetComponent<PlayerGroundCheck>();
        playerStamina = GetComponent<PlayerStamina>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAudio = GetComponent<PlayerAudio>();
    }
}
