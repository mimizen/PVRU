using UnityEngine;

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
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
    {0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0},
    {0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0},
    {0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
    {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 0},
    {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0},
    {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0},
    {0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0},
    {0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0},
    {0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0},
    {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
   
};

public GameObject trackPrefab;
    public GameObject barrierPrefab;
    public float scale = 10f;

    void Start()
    {
        GenerateTrack();
    }

   void GenerateTrack()
    {
        int rows = trackMatrix.GetLength(0);
        int cols = trackMatrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (trackMatrix[i, j] == 1)
                {
                    Vector3 position = new Vector3(j * scale, 0, i * scale);
                    Instantiate(trackPrefab, position, Quaternion.identity);

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
            Instantiate(barrierPrefab, position + new Vector3(-scale / 2, 0, 0), Quaternion.Euler(0, 90, 0));
        }
        if (j == cols - 1 || trackMatrix[i, j + 1] == 0)
        {
            Instantiate(barrierPrefab, position + new Vector3(scale / 2, 0, 0), Quaternion.Euler(0, 90, 0));
        }
        if (i == 0 || trackMatrix[i - 1, j] == 0)
        {
            Instantiate(barrierPrefab, position + new Vector3(0, 0, -scale / 2), Quaternion.identity);
        }
        if (i == rows - 1 || trackMatrix[i + 1, j] == 0)
        {
            Instantiate(barrierPrefab, position + new Vector3(0, 0, scale / 2), Quaternion.identity);
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
}