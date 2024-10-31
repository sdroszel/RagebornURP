using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIChase : MonoBehaviour
{
    public enum AISTATE {PATROL=0, CHASE=1, ATTACK=2}
    private NavMeshAgent ThisAgent = null;
    private Transform Player = null;
    public AISTATE CurrentState = AISTATE.PATROL;

    public float RotationAmount = 2f;
    public int TicksPerSecond = 60;
    public bool Pause = false;
    public float Speed = 1f;

    private void Awake() {
        ThisAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void Start() {
        ChangeState(AISTATE.PATROL);
        StartCoroutine(StatePatrol());
    }

    private IEnumerator Rotate() {
        WaitForSeconds Wait = new WaitForSeconds(1f/TicksPerSecond);

        while (true) {
            if (!Pause) {
                transform.Rotate(Vector3.up * RotationAmount);
            }

            yield return Wait;
        } 
    }

    public void ChangeState(AISTATE NewState) {
        StopAllCoroutines();
        CurrentState = NewState;

        switch(CurrentState) {
            case AISTATE.PATROL:
                StartCoroutine(StatePatrol());
                break;
            
            case AISTATE.CHASE:
                StartCoroutine(StateChase());
                break;

            case AISTATE.ATTACK:
                StartCoroutine(StateAttack());
                break;
        }
    }

    public IEnumerator StateChase() {
        float AttackDistance = 2f;
        
        while(CurrentState == AISTATE.CHASE) {
            if (Vector3.Distance(transform.position, Player.transform.position) < AttackDistance) {
                ChangeState(AISTATE.ATTACK);
                yield break;
            }

            transform.LookAt(Player.transform);
            ThisAgent.SetDestination(Player.transform.position);
            yield return null;
        }
    }

    public IEnumerator StateAttack() {
        float AttackDistance = 2f;
        
        while (CurrentState == AISTATE.ATTACK) {
            if (Vector3.Distance(transform.position, Player.transform.position) > AttackDistance) {
                ChangeState(AISTATE.CHASE);
            }
        
            print("Attack!");
            ThisAgent.SetDestination(Player.transform.position);
            yield return null;
        }
    }

    public IEnumerator StatePatrol() {
        GameObject[] Waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        GameObject CurrentWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];
        float TargetDistance = 2f;

        while(CurrentState == AISTATE.PATROL) {
            StartCoroutine(Rotate());
            Quaternion lookRotation = Quaternion.LookRotation(CurrentWaypoint.transform.position - transform.position);

            float time = 0;

            while (time < 1) {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

                time += Time.deltaTime * Speed;

                yield return null; 
            }
            
            transform.LookAt(CurrentWaypoint.transform);
            ThisAgent.SetDestination(CurrentWaypoint.transform.position);

            if (Vector3.Distance(transform.position, CurrentWaypoint.transform.position) < TargetDistance) {
                CurrentWaypoint = Waypoints[Random.Range(0, Waypoints.Length)];
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        ChangeState(AISTATE.CHASE);
    }

}
