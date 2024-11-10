using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
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
    [SerializeField] private float chaseSpeed = 3.5f;

    [Header("Reference to Player")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerController playerController;

    [Header("Player Detection Settings")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float fovRange = 15f;
    [SerializeField] private float lineOfSightAngle = 45f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int attackDamage = 10;

    [Header("Weapon Collider Reference")]
    [SerializeField] private Collider weaponCollider;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool isChasing = false;
    private bool canAttack = true;
    private bool isDead = false;
    private Animator animator;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        weaponCollider.enabled = false;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDead) return;

        if (playerController.playerHealth.IsDead)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", true);
            MoveToWaypoint();
        }

        DetectPlayer();

        if (isChasing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && canAttack)
            {
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                StartCoroutine(AttackPlayer());
            }
            else if (distanceToPlayer > attackRange)
            {
                ChasePlayer();
            }
        }
        else if (!isWaiting)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", true);
            MoveToWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        if (isDead) return;
        animator.SetBool("isWalking", true);
        Waypoint targetWaypoint = waypoints[currentWaypointIndex];

        Vector3 direction = (targetWaypoint.transform.position - transform.position).normalized;
        direction.y = 0;

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
        if (playerController.playerHealth.IsDead) return;
        if (isDead) return;
        if (player == null)
        {
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
                else
                {
                    playerInSight = false;
                }
            }
        }

        isChasing = playerInProximity || playerInSight;
    }



    private void ChasePlayer()
    {
        if (playerController.playerHealth.IsDead) return;
        if (isDead) return;
        animator.SetBool("isRunning", true);
        animator.SetBool("isWalking", false);

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator AttackPlayer()
    {
        if (isDead || playerController.playerHealth.IsDead) yield break;
        canAttack = false;
        animator.SetTrigger("Attack");

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        float attackDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        // Rotate toward the player for the duration of the attack
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration)
        {
            FacePlayer();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void FacePlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }


    public void EnableWeaponCollider()
    {
        if (!isDead)
        {
            weaponCollider.enabled = true;
        }
    }

    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && weaponCollider.enabled && !isDead)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
        }
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

        weaponCollider.enabled = false;
        if (audioSource != null) audioSource.Stop();

        isChasing = false;
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
