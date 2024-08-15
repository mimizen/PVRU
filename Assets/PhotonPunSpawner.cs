using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class PhotonPunSpawner : MonoBehaviourPun
{
    public List<GameObject> obstaclePrefabs; // List of obstacle prefabs to spawn
    public List<GameObject> powerUpPrefabs; // List of power-up prefabs to spawn
    public float offsetRange = 5f; // Offset range for spawning
    public float minSpacing = 2f; // Minimum spacing required between objects on the x-axis
    public int obstacleSpawnAmount = 5; // Total amount of obstacles to spawn across all checkpoints
    public int powerUpSpawnAmount = 3; // Total amount of power-ups to spawn across all checkpoints

    private TrackRegenerator trackRegenerator;
    private List<Vector3> spawnPositions;
    private List<Vector3> usedPositions; // List to track used positions to avoid overlaps

    private void Start()
    {
        trackRegenerator = FindObjectOfType<TrackRegenerator>();

        // Register to the UnityEvent
        trackRegenerator.OnNewCheckpointListGenerated.AddListener(SpawnObjectsAtNewCheckpoints);

        // Wait for 2 seconds, then pull the first set of coordinates and spawn objects if master
        StartCoroutine(SpawnInitialObjects());
    }

    private IEnumerator SpawnInitialObjects()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Pull the first set of coordinates and spawn objects if this is the master client
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPositions = trackRegenerator.GetNewCheckpointsPosition();
            usedPositions = new List<Vector3>(); // Initialize the used positions list
            SpawnObjectsAtPositions(spawnPositions);
        }
    }

    private void SpawnObjectsAtNewCheckpoints()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPositions = trackRegenerator.GetNewCheckpointsPosition();
            usedPositions = new List<Vector3>(); // Reset the used positions list
            SpawnObjectsAtPositions(spawnPositions);
        }
    }

    private void SpawnObjectsAtPositions(List<Vector3> positions)
    {
        int totalCheckpoints = positions.Count;
        if (totalCheckpoints == 0) return;

        // Spawn Obstacles
        for (int i = 0; i < obstacleSpawnAmount; i++)
        {
            // Select a random position from the list that hasn't been used and respects the minimum spacing
            Vector3 selectedPosition = GetRandomAvailablePosition(positions);

            if (selectedPosition != Vector3.zero) // If a valid position was found
            {
                Vector3 spawnPosition = selectedPosition + GetRandomOffset();
                PhotonNetwork.Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)].name, spawnPosition, Quaternion.identity);
                usedPositions.Add(spawnPosition);
            }
        }

        // Spawn Power-Ups
        for (int i = 0; i < powerUpSpawnAmount; i++)
        {
            // Select a random position from the list that hasn't been used and respects the minimum spacing
            Vector3 selectedPosition = GetRandomAvailablePosition(positions);

            if (selectedPosition != Vector3.zero) // If a valid position was found
            {
                Vector3 spawnPosition = selectedPosition + GetRandomOffset();
                GameObject powerUp = PhotonNetwork.Instantiate(powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)].name, spawnPosition, Quaternion.identity);
                powerUp.SetActive(true);
                usedPositions.Add(spawnPosition);
            }
        }
    }

    private Vector3 GetRandomAvailablePosition(List<Vector3> positions)
    {
        List<Vector3> shuffledPositions = new List<Vector3>(positions);
        shuffledPositions.Shuffle(); // Shuffle positions to ensure randomness

        foreach (Vector3 position in shuffledPositions)
        {
            if (IsPositionAvailable(position))
            {
                return position;
            }
        }

        return Vector3.zero; // Return zero vector if no available position found
    }

    private bool IsPositionAvailable(Vector3 position)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Mathf.Abs(position.x - usedPosition.x) < minSpacing)
            {
                return false; // Position is too close to a used position on the x-axis
            }
        }
        return true; // Position is available
    }

    private Vector3 GetRandomOffset()
    {
        return new Vector3(
            Random.Range(-offsetRange, offsetRange), // Offset on x-axis
            0,                                      // No offset on y-axis
            0                                       // No offset on z-axis
        );
    }
}

// Extension method to shuffle a list
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
