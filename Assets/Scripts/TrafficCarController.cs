using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class TrafficCarController : MonoBehaviour
{
    private NavMeshAgent agent;
    public List<Transform> waypoints; // List of all waypoints forming the circuit
    private int currentWaypointIndex = 0;
    public float laneOffset = 2.0f; // Offset to the right or left from the path
    public bool offsetRight = true; // Direction of the offset

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        UpdateDestination();
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            MoveToNextWaypoint();
        }

        ApplyLaneOffset();
    }

    void UpdateDestination()
    {
        if (waypoints.Count == 0) return;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    void MoveToNextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        UpdateDestination();
    }

    void ApplyLaneOffset()
    {
        if (waypoints.Count == 0) return;

        Vector3 offsetDirection = offsetRight ? transform.right : -transform.right;
        Vector3 waypointDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Vector3 targetPosition = waypoints[currentWaypointIndex].position + offsetDirection * laneOffset;

        // Adjust the agent's destination with the offset
        agent.SetDestination(targetPosition);
    }
}
