using System.Collections.Generic;
using UnityEngine;

public class Track2Generator : MonoBehaviour
{
    public List<GameObject> trackPrefabs;  // Different types of track segments
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private const float trackLength = 10f;  // Adjust based on your model size
    private int seed;

    void Start()
    {
        seed = Random.Range(0, 10000);
        lastPosition = transform.position;
        lastRotation = Quaternion.identity;
        for (int i = 0; i < 10; i++)
        {
            AddTrack(true);
        }
    }

    void Update()
    {
        if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, lastPosition) < 100)
        {
            AddTrack(false);
        }
    }

    void AddTrack(bool initial)
    {
        Vector3 positionOffset = lastRotation * new Vector3(0, 0, trackLength);
        float noiseX = Mathf.PerlinNoise(seed + lastPosition.x * 0.1f, seed + lastPosition.z * 0.1f);
        float noiseY = Mathf.PerlinNoise(seed + lastPosition.z * 0.1f, seed + lastPosition.x * 0.1f);
        float height = Mathf.Lerp(-5f, 5f, noiseX);  // Example height range
        float angle = Mathf.Lerp(-30f, 30f, noiseY);  // Example curve angle

        GameObject prefab = trackPrefabs[Random.Range(0, trackPrefabs.Count)];
        Quaternion rotation = Quaternion.Euler(0, angle, 0) * lastRotation;
        GameObject newTrack = Instantiate(prefab, lastPosition + positionOffset + new Vector3(0, height, 0), rotation);
        lastPosition = newTrack.transform.position;
        lastRotation = rotation;

        if (!initial) {
            ManagePool(newTrack);  // Handle pooling if not initial generation
        }
    }

    void ManagePool(GameObject newTrack)
    {
        // Implement pooling logic here
    }
}
