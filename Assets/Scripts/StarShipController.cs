using UnityEngine;
using System.Collections.Generic;

public class SpaceShipController : MonoBehaviour
{
    public float speed = 20f; // Base speed of the spaceship
    public float maneuverability = 5f; // The range of maneuverability (left and right)
    public float wiggleAmount = 0.5f; // The amount of wiggle applied
    public float wiggleSpeed = 2f; // The speed of the wiggle effect
    public float rayOffset = 1f; // Offset for the raycast from the spaceship's center

    public float scale = 10f; // Scale of the track, used for calculating maneuver range
    public Rigidbody shipRigidbody; // Reference to the ship's Rigidbody component
    public TrackGenerator trackGenerator;

    private List<Vector2Int> waypoints;
    private int currentWaypointIndex;
    private Vector3 targetPosition;
    public float currentOffset = 0f; // This allows for lateral movement

    void Start()
    {
        // Get and initialize the waypoints
        waypoints = trackGenerator.GetSortedWaypoints();
        waypoints.RemoveAt(0); 
        currentWaypointIndex = 0;

        // Position the ship at the first waypoint
        transform.position = new Vector3(waypoints[currentWaypointIndex].y * trackGenerator.scale, 0, waypoints[currentWaypointIndex].x * trackGenerator.scale);

        // Set the initial target position
        UpdateTargetPosition();
    }

    void Update()
    {
        HandleManeuver();
        MoveAlongPath();
        ApplyWiggle();

        // Perform the raycast in the direction of the next waypoint with the offset
        RaycastToNextWaypoint();
    }

    void HandleManeuver()
    {
        // Assuming player control for offset; replace this with AI logic as needed
        // float maneuverInput = Input.GetAxis("Horizontal");
        // currentOffset = Mathf.Clamp(maneuverInput * maneuverability, -maneuverability, maneuverability);

        // Update the target position with the new offset in real-time
        UpdateTargetPosition();
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

        // Adjust the ship's rotation to look toward the target position
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.5f);
    }

    void UpdateTargetPosition()
    {
        Vector2Int currentWaypoint = waypoints[currentWaypointIndex];
        Vector3 position = new Vector3(currentWaypoint.y * scale, 0, currentWaypoint.x * scale);

        if (currentWaypointIndex < waypoints.Count - 1)
        {
            Vector2Int nextWaypoint = waypoints[(currentWaypointIndex + 1) % waypoints.Count];
            Vector2Int nextNextWaypoint = waypoints[(currentWaypointIndex + 2) % waypoints.Count];

            // Interpolate between the next waypoint and the one after it
            Vector3 nextPosition = new Vector3(nextWaypoint.y * scale, 0, nextWaypoint.x * scale);
            Vector3 nextNextPosition = new Vector3(nextNextWaypoint.y * scale, 0, nextNextWaypoint.x * scale);

            // Linear interpolation between the next two waypoints (t = 0.5 for midpoint)
            Vector3 interpolatedPosition = Vector3.Lerp(nextPosition, nextNextPosition, 0.5f);
            Vector3 direction = (interpolatedPosition - position).normalized;

            // Apply the offset consistently to the target position
            Vector3 offsetDirection = Quaternion.Euler(0, 90, 0) * direction * currentOffset;
            targetPosition = interpolatedPosition + offsetDirection;
        }
    }

    void ApplyWiggle()
    {
        float wiggle = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAmount;
        shipRigidbody.transform.localPosition += new Vector3(0, wiggle, 0);
    }

    void RaycastToNextWaypoint()
    {
        // Start position of the raycast, adjusted by the rayOffset
        Vector3 rayStartPos = transform.position + transform.right * rayOffset;

        // Calculate the direction to the next waypoint
        Vector3 directionToWaypoint = (targetPosition - rayStartPos).normalized;

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(rayStartPos, directionToWaypoint, out hit))
        {
            Debug.DrawLine(rayStartPos, hit.point, Color.red); // Draw the ray in the editor for visualization

            // Log the name of the object hit by the raycast (or take any other action)
            Debug.Log("Raycast hit: " + hit.collider.name);
        }
        else
        {
            Debug.DrawLine(rayStartPos, rayStartPos + directionToWaypoint * 100f, Color.green); // Draw the ray if nothing is hit
        }
    }
}
