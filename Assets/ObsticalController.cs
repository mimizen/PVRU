using UnityEngine;
using System.Collections.Generic;

public class ObstacleController : MonoBehaviour
{
    public TrackGenerator trackGenerator; // Reference to the TrackGenerator script
    public float speed = 5f; // Speed at which the obstacle moves
    public int startPosition = 0; // Starting position in the waypoint list
    public float offset = 1f; // Offset to the left or right based on the heading

    [SerializeField]
    private List<Vector2Int> waypoints; // Serialized field to show in the Inspector

    private int currentWaypointIndex;
    private Vector3 targetPosition;

    void Start()
    {
        waypoints = trackGenerator.GetSortedWaypoints();
        waypoints.RemoveAt(0); // Remove the first waypoint as it is the starting position
        currentWaypointIndex = startPosition % waypoints.Count;
        this.transform.position = new Vector3(waypoints[currentWaypointIndex].y * trackGenerator.scale, 0, waypoints[currentWaypointIndex].x * trackGenerator.scale);
        UpdateTargetPosition();
    }

    void Update()
    {
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            UpdateTargetPosition();
        }
    }

    void UpdateTargetPosition()
    {
        Vector2Int waypoint = waypoints[currentWaypointIndex];
        Vector3 position = new Vector3(waypoint.y * trackGenerator.scale, 0, waypoint.x * trackGenerator.scale);
        Vector3 direction = Vector3.zero;

        if (currentWaypointIndex < waypoints.Count - 1)
        {
            Vector2Int nextWaypoint = waypoints[(currentWaypointIndex + 1) % waypoints.Count];
            Vector3 nextPosition = new Vector3(nextWaypoint.y * trackGenerator.scale, 0, nextWaypoint.x * trackGenerator.scale);
            direction = (nextPosition - position).normalized;
        }
        else if (waypoints.Count > 1)
        {
            Vector2Int nextWaypoint = waypoints[0];
            Vector3 nextPosition = new Vector3(nextWaypoint.y * trackGenerator.scale, 0, nextWaypoint.x * trackGenerator.scale);
            direction = (nextPosition - position).normalized;
        }

        Vector3 offsetDirection = Quaternion.Euler(0, 90, 0) * direction * offset;
        targetPosition = position + offsetDirection;
    }
}
