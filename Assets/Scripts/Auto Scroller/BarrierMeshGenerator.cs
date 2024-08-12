using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BarrierMeshGenerator : MonoBehaviour
{
    public Transform[] controlPoints; // Points to define the path
    public float barrierHeight = 2f; // Height of the barrier
    public float barrierWidth = 0.5f; // Thickness of the barrier
    public int resolution = 10; // Number of segments per unit length for smoothness

    private MeshFilter meshFilter;
    private Mesh barrierMesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GenerateBarrierMesh();
    }

    void GenerateBarrierMesh()
    {
        List<Vector3> splinePoints = GenerateSplinePoints(); // Generate points along the spline

        // Create arrays for mesh data
        Vector3[] vertices = new Vector3[splinePoints.Count * 4]; // 4 vertices per segment (2 top, 2 bottom)
        int[] triangles = new int[(splinePoints.Count - 1) * 12]; // 12 indices per segment (2 triangles per face, 6 faces per segment)

        for (int i = 0; i < splinePoints.Count; i++)
        {
            Vector3 forward = (i == splinePoints.Count - 1) ? splinePoints[i] - splinePoints[i - 1] : splinePoints[i + 1] - splinePoints[i];
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized * barrierWidth;

            // Define vertices for each segment
            vertices[i * 4] = splinePoints[i] - right;                   // Bottom left
            vertices[i * 4 + 1] = splinePoints[i] + right;               // Bottom right
            vertices[i * 4 + 2] = splinePoints[i] - right + Vector3.up * barrierHeight; // Top left
            vertices[i * 4 + 3] = splinePoints[i] + right + Vector3.up * barrierHeight; // Top right

            if (i < splinePoints.Count - 1)
            {
                int vertIndex = i * 4;
                int triIndex = i * 12;

                // Front face
                triangles[triIndex] = vertIndex;
                triangles[triIndex + 1] = vertIndex + 2;
                triangles[triIndex + 2] = vertIndex + 4;

                triangles[triIndex + 3] = vertIndex + 2;
                triangles[triIndex + 4] = vertIndex + 6;
                triangles[triIndex + 5] = vertIndex + 4;

                // Back face
                triangles[triIndex + 6] = vertIndex + 1;
                triangles[triIndex + 7] = vertIndex + 5;
                triangles[triIndex + 8] = vertIndex + 3;

                triangles[triIndex + 9] = vertIndex + 3;
                triangles[triIndex + 10] = vertIndex + 5;
                triangles[triIndex + 11] = vertIndex + 7;
            }
        }

        // Create the mesh
        barrierMesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };

        barrierMesh.RecalculateNormals(); // Ensure correct shading
        meshFilter.mesh = barrierMesh; // Assign the generated mesh to the MeshFilter
    }

    List<Vector3> GenerateSplinePoints()
    {
        // Generate spline points from control points
        List<Vector3> splinePoints = new List<Vector3>();

        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Vector3 p0 = controlPoints[i].position;
            Vector3 p1 = controlPoints[(i + 1) % controlPoints.Length].position;

            // Add interpolation points between p0 and p1 based on the resolution
            for (float t = 0; t <= 1; t += 1f / resolution)
            {
                Vector3 pointOnSpline = Vector3.Lerp(p0, p1, t);
                splinePoints.Add(pointOnSpline);
            }
        }

        return splinePoints;
    }
}
