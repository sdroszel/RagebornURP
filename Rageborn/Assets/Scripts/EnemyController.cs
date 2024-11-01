using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 10f;      // Sphere radius
    [SerializeField] private float fovRange = 15f;            // Cone range
    [SerializeField] private float lineOfSightAngle = 45f;    // FOV angle

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private bool playerInProximity = false;
    private bool playerInSight = false;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        DetectPlayer();
        if (!isWaiting && !playerInSight && !playerInProximity)
        {
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

        // Calculate positions and directions
        Vector3 enemyPosition = transform.position + Vector3.up * 1f;
        Vector3 playerPosition = player.position + Vector3.up * 1f;
        Vector3 directionToPlayer = (playerPosition - enemyPosition).normalized;

        // Check proximity detection (sphere)
        float distanceToPlayer = Vector3.Distance(enemyPosition, playerPosition);
        if (distanceToPlayer <= detectionRange) {
            playerInProximity = true;
            Debug.Log("Player detected within proximity sphere.");
        } else {
            playerInProximity = false;
        }

        // Check front cone detection (field of view)
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (distanceToPlayer <= fovRange && angleToPlayer <= lineOfSightAngle / 2) {
            RaycastHit hit;
            if (Physics.Raycast(enemyPosition, directionToPlayer, out hit, fovRange)) {
                if (hit.transform == player) {
                    playerInSight = true;
                    Debug.Log("Player detected within front cone.");
                    animator.SetBool("isWalking", false); // Stop moving when player detected in front cone
                    return;
                } else {
                    Debug.Log("Obstacle between enemy and player: " + hit.transform.name);
                }
            }
        } else {
            playerInSight = false;
        }
    }

    private void OnDrawGizmosSelected() {
        // Draw the proximity detection range as a solid sphere
        Gizmos.color = new Color(0, 1, 0, 0.2f); // Green with transparency
        Gizmos.DrawSphere(transform.position, detectionRange);

        // Draw a wireframe for the proximity detection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw the field of view cone range
        Gizmos.color = Color.red;
        Vector3 leftBoundary = Quaternion.Euler(0, -lineOfSightAngle / 2, 0) * transform.forward * fovRange;
        Vector3 rightBoundary = Quaternion.Euler(0, lineOfSightAngle / 2, 0) * transform.forward * fovRange;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        //Gizmos.DrawWireSphere(transform.position, fovRange);
    }
}
