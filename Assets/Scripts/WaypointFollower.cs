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
    public float speedThreshold = 0.5f; // Speed threshold to detect if the vehicle is stuck
    public float lowSpeedTimeLimit = 3f; // Time limit in seconds to reset after low speed
    public float wrongDirectionTimeLimit = 3f;
    
    public float push; // Time limit in seconds to reset after going in the wrong direction

    private Rigidbody rb;
    private TrackRegenerator trackRegenerator;
    private float lowSpeedTimer = 0f;
    private float wrongDirectionTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get the TrackRegenerator and subscribe to the event
        trackRegenerator = FindObjectOfType<TrackRegenerator>();
        if (trackRegenerator != null)
        {
            trackRegenerator.OnNewCheckpointListGenerated.AddListener(UpdateWaypointsList);
            UpdateWaypointsList(); // Initialize waypoints on start
        }
        else
        {
            Debug.LogError("TrackRegenerator not found!");
        }
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        MoveTowardsWaypoint();
        CheckForLowSpeed();
        CheckForWrongDirection();
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
        if(push > 0){
       rb.MovePosition(transform.position + transform.forward * push * Time.deltaTime);
    }

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

    private void CheckForLowSpeed()
    {
        // Check if the speed is below the threshold
        if (rb.velocity.magnitude < speedThreshold)
        {
            lowSpeedTimer += Time.deltaTime;

            // If the speed has been low for too long, reset to the next waypoint
            if (lowSpeedTimer > lowSpeedTimeLimit)
            {
                MoveToNextWaypoint();
                lowSpeedTimer = 0f; // Reset the timer
            }
        }
        else
        {
            lowSpeedTimer = 0f; // Reset the timer if the speed is above the threshold
        }
    }

    private void CheckForWrongDirection()
    {
        // Calculate the direction towards the next waypoint
        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        
        // Check the dot product between the vehicle's forward direction and the direction to the waypoint
        float dotProduct = Vector3.Dot(transform.forward, targetDirection);

        // If the dot product is negative, it means the vehicle is facing away from the waypoint
        if (dotProduct < 0)
        {
            wrongDirectionTimer += Time.deltaTime;

            // If the vehicle has been moving in the wrong direction for too long, reset it
            if (wrongDirectionTimer > wrongDirectionTimeLimit)
            {
                MoveToNextWaypoint();
                wrongDirectionTimer = 0f; // Reset the timer
            }
        }
        else
        {
            wrongDirectionTimer = 0f; // Reset the timer if the vehicle is moving in the right direction
        }
    }

    private void MoveToNextWaypoint()
    {
        // Move to the next waypoint
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            currentWaypointIndex = 0; // Loop back to the first waypoint
        }

        // Move the car to the next waypoint position
        transform.position = waypoints[currentWaypointIndex].position;
        rb.velocity = Vector3.zero; // Stop the car's current momentum
        transform.LookAt(waypoints[(currentWaypointIndex + 1) % waypoints.Count]); // Look towards the next waypoint
    }

    private void UpdateWaypointsList()
    {
        if (trackRegenerator != null)
        {
            waypoints = new List<Transform>();
            foreach (var checkpointArray in trackRegenerator.GetWholeCheckpointsList())
            {
                waypoints.AddRange(checkpointArray); // Flatten the list of checkpoint arrays into a single list
            }

            if (waypoints.Count > 0)
            {
                FindClosestWaypoint();
            }
        }
    }

    private void FindClosestWaypoint()
    {
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < waypoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentWaypointIndex = i;
            }
        }
    }
}
