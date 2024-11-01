using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fovRange = 15f;
    [SerializeField] private float lineOfSightAngle = 45f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private Collider weaponCollider;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool isChasing = false;
    private bool canAttack = true;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        weaponCollider.enabled = false; // Disable the weapon collider initially
    }

    private void Update() {
        DetectPlayer();

        if (isChasing) {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && canAttack) {
                // Stop moving and start attacking
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                StartCoroutine(AttackPlayer());
            } else if (distanceToPlayer > attackRange) {
                // Continue chasing if the player is out of attack range
                ChasePlayer();
            }
        } else if (!isWaiting) {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", true);
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint() {
        animator.SetBool("isWalking", true);
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        direction.y = 0;

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);
        
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f) {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint() {
        animator.SetBool("isWalking", false);
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }

    private void DetectPlayer() {
        if (player == null) {
            Debug.LogWarning("Player not assigned in EnemyController.");
            return;
        }

        Vector3 enemyPosition = transform.position + Vector3.up * 1f;
        Vector3 playerPosition = player.position + Vector3.up * 1f;
        Vector3 directionToPlayer = (playerPosition - enemyPosition).normalized;

        float distanceToPlayer = Vector3.Distance(enemyPosition, playerPosition);
        bool playerInProximity = distanceToPlayer <= detectionRange;

        bool playerInSight = false;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (distanceToPlayer <= fovRange && angleToPlayer <= lineOfSightAngle / 2) {
            RaycastHit hit;
            if (Physics.Raycast(enemyPosition, directionToPlayer, out hit, fovRange)) {
                if (hit.transform == player) {
                    playerInSight = true;
                }
            }
        }

        isChasing = playerInProximity || playerInSight;
    }

    private void ChasePlayer() {
        // Activate running animation and move towards the player if not in attack range
        animator.SetBool("isRunning", true);
        animator.SetBool("isWalking", false);

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator AttackPlayer() {
        canAttack = false;
        animator.SetTrigger("Attack"); // Trigger the attack animation

        yield return new WaitForSeconds(0.5f); // Adjust to match the hit timing in the animation

        // Apply damage if the player is still within attack range
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange) {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(attackDamage);
            }
        }

        // Cooldown before the next attack
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void EnableWeaponCollider() {
        weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider() {
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && weaponCollider.enabled) {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Vector3 gizmoPosition = transform.position + Vector3.up * 0.5f;
        
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawSphere(gizmoPosition, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(gizmoPosition, detectionRange);

        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -lineOfSightAngle / 2, 0) * transform.forward * fovRange;
        Vector3 rightBoundary = Quaternion.Euler(0, lineOfSightAngle / 2, 0) * transform.forward * fovRange;
        Gizmos.DrawLine(gizmoPosition, gizmoPosition + leftBoundary);
        Gizmos.DrawLine(gizmoPosition, gizmoPosition + rightBoundary);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gizmoPosition, attackRange);
    }
}
