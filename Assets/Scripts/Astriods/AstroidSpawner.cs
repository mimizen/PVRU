using UnityEngine;
using Photon.Pun;
using System.Collections;

public class AsteroidSpawner : MonoBehaviourPun
{
    public GameObject asteroidPrefab; // The asteroid prefab to spawn
    public TrackRegenerator trackRegenerator;
    public Vector3 offset; // Offset from the checkpoint where the asteroid will spawn
    public bool spawnContinuously = false; // Toggle to spawn continuously every 2 seconds
    public Transform asteroidParent; // Parent object to hold all spawned asteroids

    void Start()
    {
        if (asteroidParent == null)
        {
            asteroidParent = new GameObject("Asteroids").transform;
        }

        if (spawnContinuously)
        {
            StartCoroutine(SpawnAsteroidsContinuously());
        }
    }

    void SpawnAsteroid()
    {
        if (PhotonNetwork.IsMasterClient) // Ensure only the master client spawns the asteroid
        {
            Transform lastCheckpoint = trackRegenerator.GetLastCheckpointOfFrontTrack();
            Transform[] checkpoints = trackRegenerator.GetCheckpointsOfFrontTrack();

            if (lastCheckpoint != null && checkpoints != null)
            {
                GameObject asteroid = PhotonNetwork.Instantiate(asteroidPrefab.name, lastCheckpoint.position + offset, Quaternion.identity);
                asteroid.GetComponent<Asteroid>().Initialize(checkpoints);

                // Set the asteroid's parent to keep the hierarchy organized
                asteroid.transform.SetParent(asteroidParent);
            }
            else
            {
                Debug.LogError("Failed to spawn asteroid: lastCheckpoint or checkpoints are null.");
            }
        }
    }

    IEnumerator SpawnAsteroidsContinuously()
    {
        while (spawnContinuously)
        {
            SpawnAsteroid();
            yield return new WaitForSeconds(0.5f); // Wait for 2 seconds before spawning the next asteroid
        }
    }
}
