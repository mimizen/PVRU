using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TrackAndBarrierMeshGenerator : MonoBehaviour
{
    public Transform[] controlPoints; // Points to define the path
    public GameObject checkpointPrefab; // Prefab for checkpoints
    public float trackWidth = 5f; // Width of the track
    public float barrierHeight = 2f; // Height of the barrier
    public float barrierWidth = 0.5f; // Thickness of the barrier
    public int resolution = 10; // Number of segments per unit length for smoothness
    public string pathString = "RSSRLLSR"; // String defining the path

    public MeshFilter trackMeshFilter;
    public MeshFilter barrierMeshFilter;
    public MeshCollider trackMeshCollider;
    public MeshCollider barrierMeshCollider;

    private Mesh trackMesh;
    private Mesh barrierMesh;

    private GameObject trackWithBarrier; // Parent GameObject

    void Start()
    {
        trackWithBarrier = new GameObject("TrackWithBarrier"); // Create parent GameObject
        GenerateTrackAndBarrierMeshes();
        GenerateCheckpoints();
        SaveMeshesAsAssets();
    }

    void GenerateTrackAndBarrierMeshes()
    {
        List<Vector3> splinePoints = GenerateSplinePoints(); // Generate points along the spline

        // Create arrays for track mesh data
        Vector3[] trackVertices = new Vector3[splinePoints.Count * 4]; // 4 vertices per segment (2 for top, 2 for bottom)
        int[] trackTriangles = new int[(splinePoints.Count - 1) * 6]; // 6 indices per segment (2 triangles)

        // Create arrays for barrier mesh data
        Vector3[] barrierVertices = new Vector3[splinePoints.Count * 4]; // 4 vertices per segment (2 for left barrier, 2 for right barrier)
        int[] barrierTriangles = new int[(splinePoints.Count - 1) * 12]; // 12 indices per segment (4 triangles)

        for (int i = 0; i < splinePoints.Count; i++)
        {
            Vector3 forward = (i == splinePoints.Count - 1) ? splinePoints[i] - splinePoints[i - 1] : splinePoints[i + 1] - splinePoints[i];
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized * trackWidth / 2;

            // Track vertices (bottom left, bottom right, top left, top right)
            trackVertices[i * 4] = splinePoints[i] - right; // Bottom left of track
            trackVertices[i * 4 + 1] = splinePoints[i] + right; // Bottom right of track
            trackVertices[i * 4 + 2] = splinePoints[i] - right + Vector3.up * 0.1f; // Top left of track
            trackVertices[i * 4 + 3] = splinePoints[i] + right + Vector3.up * 0.1f; // Top right of track

            // Barrier vertices (left barrier bottom, left barrier top, right barrier bottom, right barrier top)
            barrierVertices[i * 4] = splinePoints[i] - right; // Bottom of left barrier
            barrierVertices[i * 4 + 1] = splinePoints[i] - right + Vector3.up * barrierHeight; // Top of left barrier
            barrierVertices[i * 4 + 2] = splinePoints[i] + right; // Bottom of right barrier
            barrierVertices[i * 4 + 3] = splinePoints[i] + right + Vector3.up * barrierHeight; // Top of right barrier

            if (i < splinePoints.Count - 1)
            {
                int trackVertIndex = i * 4;
                int trackTriIndex = i * 6;

                // Track surface (correct winding order)
                trackTriangles[trackTriIndex] = trackVertIndex;
                trackTriangles[trackTriIndex + 1] = trackVertIndex + 4;
                trackTriangles[trackTriIndex + 2] = trackVertIndex + 1;

                trackTriangles[trackTriIndex + 3] = trackVertIndex + 1;
                trackTriangles[trackTriIndex + 4] = trackVertIndex + 4;
                trackTriangles[trackTriIndex + 5] = trackVertIndex + 5;

                int barrierVertIndex = i * 4;
                int barrierTriIndex = i * 12;

                // Left barrier face (correct winding order)
                barrierTriangles[barrierTriIndex] = barrierVertIndex;
                barrierTriangles[barrierTriIndex + 1] = barrierVertIndex + 1;
                barrierTriangles[barrierTriIndex + 2] = barrierVertIndex + 4;

                barrierTriangles[barrierTriIndex + 3] = barrierVertIndex + 4;
                barrierTriangles[barrierTriIndex + 4] = barrierVertIndex + 1;
                barrierTriangles[barrierTriIndex + 5] = barrierVertIndex + 5;

                // Right barrier face (correct winding order)
                barrierTriangles[barrierTriIndex + 6] = barrierVertIndex + 2;
                barrierTriangles[barrierTriIndex + 7] = barrierVertIndex + 6;
                barrierTriangles[barrierTriIndex + 8] = barrierVertIndex + 3;

                barrierTriangles[barrierTriIndex + 9] = barrierVertIndex + 3;
                barrierTriangles[barrierTriIndex + 10] = barrierVertIndex + 6;
                barrierTriangles[barrierTriIndex + 11] = barrierVertIndex + 7;
            }
        }

        // Create and assign the track mesh
        trackMesh = new Mesh
        {
            vertices = trackVertices,
            triangles = trackTriangles
        };
        trackMesh.RecalculateNormals();
        trackMeshFilter.mesh = trackMesh;
        trackMeshCollider.sharedMesh = trackMesh;
        trackMeshFilter.transform.SetParent(trackWithBarrier.transform); // Parent to TrackWithBarrier

        // Create and assign the barrier mesh
        barrierMesh = new Mesh
        {
            vertices = barrierVertices,
            triangles = barrierTriangles
        };
        barrierMesh.RecalculateNormals();
        barrierMeshFilter.mesh = barrierMesh;
        barrierMeshCollider.sharedMesh = barrierMesh;
        barrierMeshFilter.transform.SetParent(trackWithBarrier.transform); // Parent to TrackWithBarrier
    }

    List<Vector3> GenerateSplinePoints()
    {
        // Generate spline points from control points
        List<Vector3> splinePoints = new List<Vector3>();
        Vector3 direction = Vector3.forward;

        for (int i = 0; i < pathString.Length; i++)
        {
            char action = pathString[i];
            Vector3 lastPoint = (splinePoints.Count > 0) ? splinePoints[splinePoints.Count - 1] : controlPoints[0].position;

            if (action == 'S')
            {
                Vector3 nextPoint = lastPoint + direction * trackWidth;
                splinePoints.Add(nextPoint);
            }
            else if (action == 'R')
            {
                direction = Quaternion.Euler(0, 22.5f, 0) * direction;
                Vector3 nextPoint = lastPoint + direction * trackWidth;
                splinePoints.Add(nextPoint);
            }
            else if (action == 'L')
            {
                direction = Quaternion.Euler(0, -22.5f, 0) * direction;
                Vector3 nextPoint = lastPoint + direction * trackWidth;
                splinePoints.Add(nextPoint);
            }
        }

        return splinePoints;
    }

    void GenerateCheckpoints()
    {
        Vector3 direction = Vector3.forward;
        Vector3 lastPosition = controlPoints[0].position;

        for (int i = 0; i < pathString.Length; i++)
        {
            char action = pathString[i];
            Vector3 checkpointPosition;

            if (action == 'S')
            {
                checkpointPosition = lastPosition + direction * trackWidth;
            }
            else if (action == 'R')
            {
                direction = Quaternion.Euler(0, 22.5f, 0) * direction;
                checkpointPosition = lastPosition + direction * trackWidth;
            }
            else if (action == 'L')
            {
                direction = Quaternion.Euler(0, -22.5f, 0) * direction;
                checkpointPosition = lastPosition + direction * trackWidth;
            }
            else
            {
                continue;
            }

            GameObject checkpoint = Instantiate(checkpointPrefab, checkpointPosition, Quaternion.LookRotation(direction));
            checkpoint.name = "Checkpoint_" + i; // Name the checkpoints sequentially
            checkpoint.transform.SetParent(trackWithBarrier.transform); // Parent to TrackWithBarrier

            lastPosition = checkpointPosition;
        }
    }

    void SaveMeshesAsAssets()
    {
        #if UNITY_EDITOR
        // Ensure a folder exists
        string folderPath = "Assets/GeneratedMeshes";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "GeneratedMeshes");
        }

        // Save track mesh
        string trackMeshPath = folderPath + "/TrackMesh.asset";
        AssetDatabase.CreateAsset(trackMesh, trackMeshPath);
        AssetDatabase.SaveAssets();

        // Save barrier mesh
        string barrierMeshPath = folderPath + "/BarrierMesh.asset";
        AssetDatabase.CreateAsset(barrierMesh, barrierMeshPath);
        AssetDatabase.SaveAssets();

        // Assign saved meshes back to filters
        trackMeshFilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(trackMeshPath);
        barrierMeshFilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(barrierMeshPath);
        #endif
    }
    public GameObject returnTrackWithBarrier()
    {
        return trackWithBarrier;
    }
}
