using UnityEngine;
using System.Collections.Generic;

public class TrackGenerator : MonoBehaviour
{
   /* public int[,] trackMatrix = new int[,] {
        {1, 1, 1, 0, 0},
        {1, 0, 1, 1, 1},
        {1, 0, 0, 0, 1},
        {1, 1, 0, 0, 1},
        {0, 1, 1, 1, 1}
    };*/
    public int[,] trackMatrix = new int[,] {
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0},
    {0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0},
    {0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0},
    {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
    {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
    {0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0},
    {0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0},
    {0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0},
    {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
   
};

    public GameObject trackPrefab;
    public GameObject barrierPrefab;
    public float scale = 10f;
    private GameObject trackParent;
    private GameObject barrierParent;

    public GameObject waypointPrefab; // Assign this prefab in the Unity Inspector
    private GameObject waypointsParent; // Parent object to hold all waypoints for organization

    void Start()
    {
        trackParent = new GameObject("TrackParent");

        barrierParent = new GameObject("BarrierParent");

        waypointsParent = new GameObject("WaypointsParent");

        trackParent.transform.SetParent(transform);

        barrierParent.transform.SetParent(transform);

        waypointsParent.transform.SetParent(transform);

        GenerateTrack();
    }
    void GenerateTrack()
    {
        int rows = trackMatrix.GetLength(0);
        int cols = trackMatrix.GetLength(1);

        Vector2Int start = new Vector2Int(1, 1); // Start position
        List<Vector2Int> path = DepthFirstSearch(start, rows, cols);

        int waypointCount = 0;
        foreach (Vector2Int pos in path)
        {
            Vector3 position = new Vector3(pos.y * scale, 0, pos.x * scale);
            GameObject trackPiece = Instantiate(trackPrefab, position, Quaternion.identity);
            trackPiece.transform.SetParent(trackParent.transform);

            GameObject waypoint = Instantiate(waypointPrefab, position, Quaternion.identity);
            waypoint.name = "Waypoint " + waypointCount;
            waypoint.transform.SetParent(waypointsParent.transform);
            waypointCount++;

            PlaceBarriers(pos.x, pos.y, position, rows, cols);
        }
    }
    List<Vector2Int> DepthFirstSearch(Vector2Int start, int rows, int cols)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        bool[,] visited = new bool[rows, cols];
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), // right
            new Vector2Int(1, 0), // down
            new Vector2Int(0, -1), // left
            new Vector2Int(-1, 0) // up
        };

        stack.Push(start);
        visited[start.x, start.y] = true;

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            path.Add(current);

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (IsValidPosition(next.x, next.y, rows, cols) && !visited[next.x, next.y] && trackMatrix[next.x, next.y] == 1)
                {
                    stack.Push(next);
                    visited[next.x, next.y] = true;
                    break; // Ensure DFS moves in one direction at a time
                }
            }
        }

        return path;
    }
    List<Vector2Int> BreadthFirstSearch(Vector2Int start, int rows, int cols)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        bool[,] visited = new bool[rows, cols];
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), // right
            new Vector2Int(1, 0), // down
            new Vector2Int(0, -1), // left
            new Vector2Int(-1, 0) // up
        };

        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            path.Add(current);

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (IsValidPosition(next.x, next.y, rows, cols) && !visited[next.x, next.y] && trackMatrix[next.x, next.y] == 1)
                {
                    queue.Enqueue(next);
                    visited[next.x, next.y] = true;
                }
            }
        }

        return path;
    }

     void GenerateTrack2()
    {
        int rows = trackMatrix.GetLength(0);
        int cols = trackMatrix.GetLength(1);
        int waypointCount = 0;

        for (int i = 0; i < rows; i++)
        {
            bool waypointPlaced = false;  // To ensure one waypoint per row
            for (int j = 0; j < cols; j++)
            {
                if (trackMatrix[i, j] == 1)
                {
                    Vector3 position = new Vector3(j * scale, 0, i * scale);
                    GameObject trackPiece = Instantiate(trackPrefab, position, Quaternion.identity);
                    trackPiece.transform.SetParent(trackParent.transform);
                    GameObject Waypoint = Instantiate(waypointPrefab, position, Quaternion.identity);
                    Waypoint.name = "Waypoint " + waypointCount;
                    Waypoint.transform.SetParent(waypointsParent.transform);
                    waypointCount++;



                    
                     PlaceBarriers(i, j, position, rows, cols);
                }
            }
        }
    }

    void PlaceBarriers(int i, int j, Vector3 position, int rows, int cols)
    {
        // Adjust this method to fine-tune where and how barriers are placed
        PlaceSideBarriers(i, j, position, rows, cols);
        //PlaceDiagonalBarriers(i, j, position, rows, cols);
    }

    void PlaceSideBarriers(int i, int j, Vector3 position, int rows, int cols)
    {
        if (j == 0 || trackMatrix[i, j - 1] == 0)
        {
             GameObject barrier = Instantiate(barrierPrefab, position + new Vector3(-scale / 2, 0, 0), Quaternion.Euler(0, 90, 0));
             barrier.transform.SetParent(barrierParent.transform);
        }
        if (j == cols - 1 || trackMatrix[i, j + 1] == 0)
        {
             GameObject barrier = Instantiate(barrierPrefab, position + new Vector3(scale / 2, 0, 0), Quaternion.Euler(0, 90, 0));
             barrier.transform.SetParent(barrierParent.transform);
        }
        if (i == 0 || trackMatrix[i - 1, j] == 0)
        {
             GameObject barrier = Instantiate(barrierPrefab, position + new Vector3(0, 0, -scale / 2), Quaternion.identity);
             barrier.transform.SetParent(barrierParent.transform);
        }
        if (i == rows - 1 || trackMatrix[i + 1, j] == 0)
        {
             GameObject barrier = Instantiate(barrierPrefab, position + new Vector3(0, 0, scale / 2), Quaternion.identity);
             barrier.transform.SetParent(barrierParent.transform);
        }
    }

      void PlaceDiagonalBarriers(int i, int j, Vector3 position, int rows, int cols)
    {
        // Correcting diagonal barrier placement, ensure no diagonal barriers are placed inappropriately
        if (ShouldPlaceDiagonalBarrier(i, j, -1, -1, rows, cols))
            Instantiate(barrierPrefab, position + new Vector3(-scale / 2, 0, -scale / 2), Quaternion.Euler(0, -45, 0));
        if (ShouldPlaceDiagonalBarrier(i, j, -1, 1, rows, cols))
            Instantiate(barrierPrefab, position + new Vector3(scale / 2, 0, -scale / 2), Quaternion.Euler(0, 45, 0));
        if (ShouldPlaceDiagonalBarrier(i, j, 1, -1, rows, cols))
            Instantiate(barrierPrefab, position + new Vector3(-scale / 2, 0, scale / 2), Quaternion.Euler(0, 135, 0));
        if (ShouldPlaceDiagonalBarrier(i, j, 1, 1, rows, cols))
            Instantiate(barrierPrefab, position + new Vector3(scale / 2, 0, scale / 2), Quaternion.Euler(0, -135, 0));
    }
    bool ShouldPlaceDiagonalBarrier(int i, int j, int di, int dj, int rows, int cols)
    {
        int ni = i + di;
        int nj = j + dj;
        if (IsValidPosition(ni, nj, rows, cols) && trackMatrix[ni, nj] == 0 &&
            ((IsValidPosition(i, nj, rows, cols) && trackMatrix[i, nj] == 0) || (IsValidPosition(ni, j, rows, cols) && trackMatrix[ni, j] == 0)))
        {
            return true;
        }
        return false;
    }
bool IsValidPosition(int i, int j, int rows, int cols)
    {
        return i >= 0 && i < rows && j >= 0 && j < cols;
    }

    bool NoTrackInAdjacent(int ni, int nj, int i, int j, int rows, int cols)
    {
        // Further checks to ensure no track tiles prevent barrier placement
        return (IsValidPosition(i, nj, rows, cols) && trackMatrix[i, nj] == 0) || (IsValidPosition(ni, j, rows, cols) && trackMatrix[ni, j] == 0);
    }
void CreateWaypoint(Vector3 position)
    {
        GameObject waypoint = new GameObject("Waypoint");
        waypoint.transform.position = position + new Vector3(0, 1, 0); // Slightly elevated above the track
        waypoint.transform.SetParent(waypointsParent.transform); // Organize waypoints under a single parent
    }
    public List<Vector2Int> GetSortedWaypoints()
    {
        int rows = trackMatrix.GetLength(0);
        int cols = trackMatrix.GetLength(1);
        Vector2Int start = new Vector2Int(2, 2); // Start position
        return DepthFirstSearch(start, rows, cols);
    }
}