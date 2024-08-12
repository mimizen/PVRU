using UnityEngine;

public class PoleSpawner : MonoBehaviour
{
    public GameObject polePrefab; // Prefab of the pole to spawn
    public TrackGenerator trackGenerator; // Reference to the TrackGenerator script
    public int length = 10; // The length over which to distribute the poles
    public int amount = 5; // The number of poles to spawn

    void Start()
    {
        SpawnPoles();
    }

    void SpawnPoles()
    {
        for (int i = 0; i < amount; i++)
        {
            // Calculate the starting position
            int startPosition = (i * length) / amount;
            Vector2Int waypoint = trackGenerator.GetSortedWaypoints()[startPosition];

            // Randomize the offset between 0, -trackGenerator.scale/3, +trackGenerator.scale/3
            float[] offsets = { 0, -trackGenerator.scale / 3f, trackGenerator.scale / 3f };
            float offset = offsets[Random.Range(0, offsets.Length)];

            // Calculate the position with offset
            Vector3 position = new Vector3(waypoint.y * trackGenerator.scale, 0, waypoint.x * trackGenerator.scale);

            if (i < amount - 1)
            {
                Vector2Int nextWaypoint = trackGenerator.GetSortedWaypoints()[startPosition + 1];
                Vector3 nextPosition = new Vector3(nextWaypoint.y * trackGenerator.scale, 0, nextWaypoint.x * trackGenerator.scale);
                Vector3 direction = (nextPosition - position).normalized;

                Vector3 offsetDirection = Quaternion.Euler(0, 90, 0) * direction * offset;
                position += offsetDirection;
            }

            // Instantiate the pole at the calculated position
            GameObject pole = Instantiate(polePrefab, position, Quaternion.identity);

            // Set the pole as a child of the spawner for organization
            pole.transform.SetParent(transform);
        }
    }
}
