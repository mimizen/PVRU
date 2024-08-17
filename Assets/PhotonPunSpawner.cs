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
    public float obstacleMinOffset = 3f; // Minimum distance required between obstacles
    public int obstacleSpawnAmount = 5; // Total amount of obstacles to spawn across all checkpoints
    public int powerUpSpawnAmount = 3; // Total amount of power-ups to spawn across all checkpoints

    private TrackRegenerator trackRegenerator;
    private List<Vector3> spawnPositions;
    private List<Vector3> usedPositions; // List to track used positions to avoid overlaps

    private Transform obstaclesParent; // Reference to the parent object for all spawned objects

    private void Start()
    {
        trackRegenerator = FindObjectOfType<TrackRegenerator>();

        // Find or create the parent object named "Obsticals"
        GameObject parentObject = GameObject.Find("Obsticals");
        if (parentObject == null)
        {
            parentObject = new GameObject("Obsticals");
        }
        obstaclesParent = parentObject.transform;

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
            Vector3 selectedPosition = GetRandomAvailablePosition(positions, obstacleMinOffset);

            if (selectedPosition != Vector3.zero) // If a valid position was found
            {
                Vector3 spawnPosition = selectedPosition + GetRandomOffset(selectedPosition);
                GameObject obstacle = PhotonNetwork.Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)].name, spawnPosition, Quaternion.identity);
                obstacle.transform.SetParent(obstaclesParent); // Set the parent to "Obsticals"
                obstacle.transform.LookAt(selectedPosition); // Make the obstacle face the checkpoint or target position
                usedPositions.Add(spawnPosition);
            }
        }

        // Spawn Power-Ups
        for (int i = 0; i < powerUpSpawnAmount; i++)
        {
            Vector3 selectedPosition = GetRandomAvailablePosition(positions, minSpacing);

            if (selectedPosition != Vector3.zero) // If a valid position was found
            {
                Vector3 spawnPosition = selectedPosition + GetRandomOffset(selectedPosition);
                GameObject powerUp = PhotonNetwork.Instantiate(powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)].name, spawnPosition, Quaternion.identity);
                powerUp.transform.SetParent(obstaclesParent); // Set the parent to "Obsticals"
                powerUp.transform.LookAt(selectedPosition); // Make the power-up face the checkpoint or target position
                powerUp.SetActive(true);
                usedPositions.Add(spawnPosition);
            }
        }
    }

    private Vector3 GetRandomAvailablePosition(List<Vector3> positions, float minDistance)
    {
        List<Vector3> shuffledPositions = new List<Vector3>(positions);
        shuffledPositions.Shuffle(); // Shuffle positions to ensure randomness

        foreach (Vector3 position in shuffledPositions)
        {
            if (IsPositionAvailable(position, minDistance))
            {
                return position;
            }
        }

        return Vector3.zero; // Return zero vector if no available position found
    }

    private bool IsPositionAvailable(Vector3 position, float minDistance)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false; // Position is too close to a used position
            }
        }
        return true; // Position is available
    }

    private Vector3 GetRandomOffset(Vector3 referencePosition)
    {
        // Calculate offset in local space relative to the reference position's direction
        Vector3 randomOffset = new Vector3(
            Random.Range(-offsetRange, offsetRange), // Offset on x-axis
            0,                                      // No offset on y-axis
            0                                       // No offset on z-axis
        );

        // Transform the offset from local to world space
        return Quaternion.LookRotation(referencePosition - transform.position) * randomOffset;
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
