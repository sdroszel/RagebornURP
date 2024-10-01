using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 1f;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!isWaiting)
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

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        for (int i = 0; i < waypoints.Length; i++) {
            if (i + 1 < waypoints.Length) {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (waypoints.Length > 1) {
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
