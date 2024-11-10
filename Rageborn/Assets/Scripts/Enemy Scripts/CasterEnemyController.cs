using System.Collections;
using UnityEngine;

public class CasterEnemyController : MonoBehaviour
{
    [System.Serializable]
    public class Waypoint
    {
        public Transform transform;
        public float waitTime = 1f;
    }

    [Header("Patrol Waypoint Settings")]
    [SerializeField] private Waypoint[] waypoints;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Reference to Player")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;

    [Header("Player Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fovRange = 15f;
    [SerializeField] private float lineOfSightAngle = 45f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float spellVelocity = 15f;
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellSpawnPoint;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip castSound;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 80;
    private int currentHealth;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool isPlayerDetected = false;
    private bool canCast = true;
    private bool isDead = false;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        if (playerController.playerHealth.IsDead)
        {
            animator.SetBool("isWalking", true);
            Patrol();
            return;
        }

        DetectPlayer();

        if (isPlayerDetected && canCast)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                animator.SetBool("isWalking", false);
                StartCoroutine(CastSpell());
            }
            else
            {
                Patrol();
            }
        }
        else if (!isWaiting && !isPlayerDetected)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (isDead || isWaiting) return;

        Waypoint targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.transform.position - transform.position).normalized;
        direction.y = 0;

        // Set the walking animation
        animator.SetBool("isWalking", true);

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.transform.position, moveSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        if (Vector3.Distance(transform.position, targetWaypoint.transform.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint(targetWaypoint.waitTime));
        }
    }

    private IEnumerator WaitAtWaypoint(float waitTime)
    {
        animator.SetBool("isWalking", false);
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        isWaiting = false;
    }

    private void DetectPlayer()
    {
        if (playerController.playerHealth.IsDead || isDead) return;

        Vector3 enemyPosition = transform.position + Vector3.up * 1f;
        Vector3 playerPosition = player.position + Vector3.up * 1f;
        Vector3 directionToPlayer = (playerPosition - enemyPosition).normalized;

        float distanceToPlayer = Vector3.Distance(enemyPosition, playerPosition);
        bool playerInProximity = distanceToPlayer <= detectionRange;

        bool playerInSight = false;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (distanceToPlayer <= fovRange && angleToPlayer <= lineOfSightAngle / 2)
        {
            int layerMask = LayerMask.GetMask("Player", "Default", "Ground");
            RaycastHit hit;
            if (Physics.Raycast(enemyPosition, directionToPlayer, out hit, fovRange, layerMask))
            {
                if (hit.transform == player)
                {
                    playerInSight = true;
                }
            }
        }

        isPlayerDetected = playerInProximity || playerInSight;
    }


    private IEnumerator CastSpell()
    {
        if (isDead || playerController.playerHealth.IsDead || !canCast) yield break;

        canCast = false;  // Prevents another cast from starting
        animator.SetTrigger("Cast");

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        if (audioSource != null && castSound != null)
        {
            audioSource.PlayOneShot(castSound);
        }

        yield return new WaitForSeconds(0.5f); // Adjust for casting animation length

        // Instantiate the spell
        GameObject spell = Instantiate(spellPrefab, spellSpawnPoint.position, spellSpawnPoint.rotation);
        Rigidbody rb = spell.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 projectileDirection = (player.position - spellSpawnPoint.position).normalized;
            rb.velocity = projectileDirection * spellVelocity;
        }

        yield return new WaitForSeconds(attackCooldown); // Cooldown before next cast
        canCast = true;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        if (currentHealth > 0)
        {
            animator.SetTrigger("Hit");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Die");

        if (audioSource != null) audioSource.Stop();

        isPlayerDetected = false;
    }

    private void OnDrawGizmosSelected()
    {
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
