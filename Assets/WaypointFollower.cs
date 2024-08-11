using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    private List<Transform> waypoints; // List of waypoints
    private int currentWaypointIndex = 0;

    public float speed = 10f; // Speed of the car
    public float turnSpeed = 1f; // Speed of turning
    public float maxSteerAngle = 20f; // Maximum steering angle for wheels
    public float maxRotationAngle = 80f; // Maximum allowed rotation angle away from the waypoint
    public float collisionTurnAngle = 30f; // Maximum turn angle after collision

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get the sorted waypoints from the TrackGenerator
        TrackGenerator trackGenerator = FindObjectOfType<TrackGenerator>();
        if (trackGenerator != null)
        {
            waypoints = trackGenerator.GetSortedWaypointList();
            if (waypoints.Count > 0)
            {
                transform.position = waypoints[currentWaypointIndex].position;
            }
        }
        else
        {
            Debug.LogError("TrackGenerator not found!");
        }
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        MoveTowardsWaypoint();
    }

    private void MoveTowardsWaypoint()
    {
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, targetDirection);

        if (angleToTarget > maxRotationAngle)
        {
            targetDirection = Vector3.RotateTowards(transform.forward, targetDirection, Mathf.Deg2Rad * maxRotationAngle, 0.0f);
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        
        // Move the car forward
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 11f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; // Loop back to the first waypoint
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Implementing a bounce back by reversing the velocity
            Vector3 bounceDirection = Vector3.Reflect(rb.velocity.normalized, collision.contacts[0].normal);
            rb.velocity = bounceDirection * speed;

            // Calculate the direction to the next waypoint
            Vector3 nextWaypointDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            
            // Calculate the new rotation by rotating towards the waypoint but limit it to 30 degrees
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, nextWaypointDirection, Mathf.Deg2Rad * collisionTurnAngle, 0.0f);
            Quaternion newRotation = Quaternion.LookRotation(newDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 5 * Time.deltaTime);
        }
    }
}
