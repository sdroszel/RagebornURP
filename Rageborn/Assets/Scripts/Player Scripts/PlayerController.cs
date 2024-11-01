using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public references for modular components, which can be accessed by other modules if needed
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerCombat playerCombat;
    [HideInInspector] public PlayerJumpAndRoll playerJumpAndRoll;
    [HideInInspector] public PlayerGroundCheck groundCheck;
    [HideInInspector] public PlayerStamina playerStamina;
    [HideInInspector] public PlayerAudio playerAudio;

    private void Awake() {
        // Initialize references to each component
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerJumpAndRoll = GetComponent<PlayerJumpAndRoll>();
        groundCheck = GetComponent<PlayerGroundCheck>();
        playerStamina = GetComponent<PlayerStamina>();
        playerAudio = GetComponent<PlayerAudio>();

        // Any other shared components can be initialized here
    }
}
