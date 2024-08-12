using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstacleController obstaclePrefab; // Prefab of the obstacle to spawn
    public TrackGenerator trackGenerator; // Reference to the TrackGenerator script
    public int length = 10; // The length over which to distribute the obstacles
    public int amount = 5; // The number of obstacles to spawn

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        for (int i = 0; i < amount; i++)
        {
            // Instantiate the obstacle
            ObstacleController obstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity);

            // Randomize speed between 10 and 50
            obstacle.speed = Random.Range(10f, 50f);

            // Calculate the starting position
            int startPosition = (i * length) / amount;
            obstacle.startPosition = startPosition;

            // Randomize the offset between 0, -trackGenerator.scale/3, +trackGenerator.scale/3
            float[] offsets = { 0, -trackGenerator.scale / 3f, trackGenerator.scale / 3f };
            obstacle.offset = offsets[Random.Range(0, offsets.Length)];

            // Set the track generator reference
            obstacle.trackGenerator = trackGenerator;

            // Set the obstacle as a child of the spawner for organization
            obstacle.transform.SetParent(transform);
        }
    }
}
