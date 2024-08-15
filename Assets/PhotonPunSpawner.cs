using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;

public class PhotonPunSpawner : MonoBehaviourPun
{
    public List<GameObject> spawnablePrefabs; // List of prefabs to spawn
    public float offsetRange = 5f; // Offset range for spawning
    public int spawnAmount = 5; // Total amount of objects to spawn across all checkpoints
    private TrackRegenerator trackRegenerator;
    private List<Vector3> spawnPositions;

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
            SpawnObjectsAtPositions(spawnPositions);
        }
    }

    private void SpawnObjectsAtNewCheckpoints()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            spawnPositions = trackRegenerator.GetNewCheckpointsPosition();
            SpawnObjectsAtPositions(spawnPositions);
        }
    }

    private void SpawnObjectsAtPositions(List<Vector3> positions)
    {
        int totalCheckpoints = positions.Count;
        if (totalCheckpoints == 0) return;

        for (int i = 0; i < spawnAmount; i++)
        {
            // Select a random position from the list
            Vector3 selectedPosition = positions[Random.Range(0, totalCheckpoints)];

            // Apply offset only on the x-axis (left to right)
            Vector3 offset = new Vector3(
                Random.Range(-offsetRange, offsetRange), // Offset on x-axis
                0,                                      // No offset on y-axis
                0                                       // No offset on z-axis
            );

            Vector3 spawnPosition = selectedPosition + offset;

            GameObject prefabToSpawn = spawnablePrefabs[Random.Range(0, spawnablePrefabs.Count)];

            // Spawn the object using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPosition, Quaternion.identity);
        }
    }
}
