using UnityEngine;
using System.Collections.Generic;

public class PathGenerator : MonoBehaviour
{
    public GameObject pathSegmentPrefab; // Prefab for each path segment
    public GameObject barrierPrefab; // Prefab for the barriers
    public Transform player1;
    public Transform player2;

    public float segmentLength = 10f; // Length of each segment
    public float trackWidth = 5f; // Width of the track

    private Queue<GameObject> pathSegments = new Queue<GameObject>();
    private string pathString = "RSSRLLSR"; // Define your path string
    private int pathIndex = 0; // Track the current position in the path string
    private Vector3 lastPosition;
    private Vector3 direction = Vector3.forward; // Initial direction is forward

    void Start()
    {
        lastPosition = transform.position;

        // Generate initial path
        for (int i = 0; i < pathString.Length; i++)
        {
            ExtendPath();
        }
    }

    void Update()
    {
        if (ShouldExtendPath())
        {
            ExtendPath();
            CleanUpPath();
        }
    }

    void ExtendPath()
    {
        // Get the current action from the path string
        char action = pathString[pathIndex];
        pathIndex = (pathIndex + 1) % pathString.Length; // Loop through the string

        // Adjust direction and calculate overlapping position
        if (action == 'R')
        {
            direction = Quaternion.Euler(0, 45, 0) * direction;
            lastPosition -= Quaternion.Euler(0, 22.5f, 0) * direction * (trackWidth / 2); // Slightly adjust last position for overlap

            // Remove left barrier (inside of the turn) from the previous segment
            RemoveInsideBarrierFromPreviousSegment("left");
        }
        else if (action == 'L')
        {
            direction = Quaternion.Euler(0, -45, 0) * direction;
            lastPosition -= Quaternion.Euler(0, -22.5f, 0) * direction * (trackWidth / 2); // Slightly adjust last position for overlap

            // Remove right barrier (inside of the turn) from the previous segment
            RemoveInsideBarrierFromPreviousSegment("right");
        }

        // Calculate new position for the next segment
        Vector3 newPosition = lastPosition + direction * segmentLength;
        GameObject newSegment = Instantiate(pathSegmentPrefab, newPosition, Quaternion.LookRotation(direction));
        newSegment.transform.localScale = new Vector3(trackWidth, newSegment.transform.localScale.y, segmentLength);
        pathSegments.Enqueue(newSegment);

        // Remove any barriers that intersect with the new track piece
        RemoveIntersectingBarriers(newSegment);

        // Create barriers on either side of the track
        Vector3 offset = Quaternion.LookRotation(direction) * Vector3.right * (trackWidth / 2 + barrierPrefab.transform.localScale.x);

        Vector3 leftBarrierPosition = newPosition - offset;
        Vector3 rightBarrierPosition = newPosition + offset;

        // Instantiate and adjust the left barrier
        GameObject leftBarrier = Instantiate(barrierPrefab, leftBarrierPosition, Quaternion.LookRotation(direction));
        AdjustBarrierIfIntersecting(leftBarrier);

        // Instantiate and adjust the right barrier
        GameObject rightBarrier = Instantiate(barrierPrefab, rightBarrierPosition, Quaternion.LookRotation(direction));
        AdjustBarrierIfIntersecting(rightBarrier);

        // Optionally parent barriers to the segment for easier cleanup
        if (leftBarrier != null) leftBarrier.transform.parent = newSegment.transform;
        if (rightBarrier != null) rightBarrier.transform.parent = newSegment.transform;

        lastPosition = newPosition;
    }

    void AdjustBarrierIfIntersecting(GameObject barrier)
    {
        float minScale = 0.1f;  // Minimum scale before deciding not to place the barrier
        float scaleStep = 0.95f;  // Finer adjustment factor
        bool intersectionDetected = true;

        while (intersectionDetected && barrier.transform.localScale.z > minScale)
        {
            Collider[] hitColliders = Physics.OverlapBox(
                barrier.transform.position, 
                barrier.transform.localScale * 0.9f, 
                barrier.transform.rotation
            );

            intersectionDetected = false;
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.gameObject != barrier)
                {
                    intersectionDetected = true;
                    barrier.transform.localScale = new Vector3(
                        barrier.transform.localScale.x,
                        barrier.transform.localScale.y,
                        barrier.transform.localScale.z * scaleStep
                    );
                    break;
                }
            }
        }

        // If the barrier's scale is too small, destroy it
        if (barrier.transform.localScale.z <= minScale)
        {
            Destroy(barrier);
        }
    }

    void RemoveInsideBarrierFromPreviousSegment(string side)
    {
        if (pathSegments.Count > 0)
        {
            GameObject previousSegment = pathSegments.Peek();
            foreach (Transform child in previousSegment.transform)
            {
                if ((side == "left" && child.localPosition.x < 0) || (side == "right" && child.localPosition.x > 0))
                {
                    Destroy(child.gameObject); // Remove the inside barrier
                    break;
                }
            }
        }
    }

    void RemoveIntersectingBarriers(GameObject newTrackPiece)
    {
        // Get the bounds of the new track piece
        Collider trackCollider = newTrackPiece.GetComponent<Collider>();
        if (trackCollider == null)
        {
            Debug.LogWarning("No Collider found on the new track piece.");
            return;
        }

        // Use the bounds of the new track piece to detect intersecting barriers
        Collider[] hitColliders = Physics.OverlapBox(
            trackCollider.bounds.center,
            trackCollider.bounds.extents,
            newTrackPiece.transform.rotation
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Barrier"))
            {
                Destroy(hitCollider.gameObject); // Remove the intersecting barrier
            }
        }
    }

    bool ShouldExtendPath()
    {
        // Check if either player is within a certain distance of the last segment
        float distance1 = Vector3.Distance(player1.position, lastPosition);
        float distance2 = Vector3.Distance(player2.position, lastPosition);

        return distance1 < segmentLength * 5 || distance2 < segmentLength * 5;
    }

    void CleanUpPath()
    {
        // Remove old segments that are far behind the players
        GameObject firstSegment = pathSegments.Peek();
        float distance1 = Vector3.Distance(player1.position, firstSegment.transform.position);
        float distance2 = Vector3.Distance(player2.position, firstSegment.transform.position);

        if (distance1 > segmentLength * 10 && distance2 > segmentLength * 10)
        {
            Destroy(pathSegments.Dequeue());
        }
    }
}
