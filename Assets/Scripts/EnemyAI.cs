using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum AIState
    {
        Patrol,
        Suspicious,
        Chase
    }

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;

    [Header("Vision Settings")]
    public float viewDistance = 15f;
    public float viewAngle = 60f;
    public LayerMask obstacleMask;

    [Header("Distance Thresholds")]
    public float suspiciousDistance = 12f;
    public float chaseDistance = 6f;
    public float loseInterestDistance = 20f;

    [Header("Speeds")]
    public float patrolSpeed = 3.5f;
    public float suspiciousSpeed = 2f;
    public float chaseSpeed = 6f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int patrolIndex;

    private AIState currentState;
    private Vector3 lastKnownPlayerPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = AIState.Patrol;
        agent.speed = patrolSpeed;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CheckLineOfSight();

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior(canSeePlayer, distanceToPlayer);
                break;

            case AIState.Suspicious:
                SuspiciousBehavior(canSeePlayer, distanceToPlayer);
                break;

            case AIState.Chase:
                ChaseBehavior(distanceToPlayer);
                break;
        }
    }

    #region State Behaviors

    void PatrolBehavior(bool canSeePlayer, float distance)
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GoToNextPatrolPoint();

        if (canSeePlayer && distance <= chaseDistance)
            SwitchState(AIState.Chase);
        else if (canSeePlayer && distance <= suspiciousDistance)
            SwitchState(AIState.Suspicious);
    }

    void SuspiciousBehavior(bool canSeePlayer, float distance)
    {
        agent.SetDestination(lastKnownPlayerPosition);

        if (canSeePlayer && distance <= chaseDistance)
            SwitchState(AIState.Chase);
        else if (distance > loseInterestDistance)
            SwitchState(AIState.Patrol);
    }

    void ChaseBehavior(float distance)
    {
        agent.SetDestination(player.position);

        if (distance > suspiciousDistance)
            SwitchState(AIState.Suspicious);
    }

    #endregion

    #region Helpers

    void SwitchState(AIState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case AIState.Patrol:
                agent.speed = patrolSpeed;
                GoToNextPatrolPoint();
                break;

            case AIState.Suspicious:
                agent.speed = suspiciousSpeed;
                lastKnownPlayerPosition = player.position;
                break;

            case AIState.Chase:
                agent.speed = chaseSpeed;
                break;
        }
    }

    bool CheckLineOfSight()
    {
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < viewAngle / 2f)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (!Physics.Raycast(transform.position + Vector3.up, dirToPlayer, distance, obstacleMask))
            {
                lastKnownPlayerPosition = player.position;
                return true;
            }
        }

        return false;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[Random.Range(0, patrolPoints.Length)].position;
    }

    #endregion
}