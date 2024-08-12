using UnityEngine;
using System.Collections.Generic;

public class SplinePathGenerator : MonoBehaviour
{
    public Transform[] controlPoints; // Points to create the spline/curve
    public GameObject trackSegmentPrefab; // Prefab for the track
    public GameObject barrierPrefab; // Prefab for the barriers

    public float segmentLength = 5f; // Distance between points on the spline
    public float trackWidth = 5f; // Width of the track

    private List<Vector3> splinePoints = new List<Vector3>(); // Points along the spline

    void Start()
    {
        GenerateSpline();
        GenerateTrackAndBarriers();
    }

    void GenerateSpline()
    {
        // You can implement a Catmull-Rom or Bezier spline algorithm here
        // Or use Unity's LineRenderer with smoothing enabled

        // Example with Catmull-Rom Spline:
        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Vector3 p0 = controlPoints[i].position;
            Vector3 p1 = controlPoints[(i + 1) % controlPoints.Length].position;

            // Add interpolation points between p0 and p1
            for (float t = 0; t <= 1; t += segmentLength / Vector3.Distance(p0, p1))
            {
                Vector3 pointOnSpline = Vector3.Lerp(p0, p1, t);
                splinePoints.Add(pointOnSpline);
            }
        }
    }

    void GenerateTrackAndBarriers()
    {
        for (int i = 0; i < splinePoints.Count - 1; i++)
        {
            Vector3 currentPoint = splinePoints[i];
            Vector3 nextPoint = splinePoints[i + 1];

            // Place track segments
            Vector3 segmentPosition = Vector3.Lerp(currentPoint, nextPoint, 0.5f);
            Quaternion segmentRotation = Quaternion.LookRotation(nextPoint - currentPoint);
            Instantiate(trackSegmentPrefab, segmentPosition, segmentRotation);

            // Place barriers
            Vector3 leftBarrierPosition = segmentPosition - segmentRotation * Vector3.right * (trackWidth / 2);
            Vector3 rightBarrierPosition = segmentPosition + segmentRotation * Vector3.right * (trackWidth / 2);

            Instantiate(barrierPrefab, leftBarrierPosition, segmentRotation);
            Instantiate(barrierPrefab, rightBarrierPosition, segmentRotation);
        }
    }
}
