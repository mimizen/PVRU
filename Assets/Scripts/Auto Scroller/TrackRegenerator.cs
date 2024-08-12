using UnityEngine;

public class TrackRegenerator : MonoBehaviour
{
    public GameObject trackPrefab; // Prefab to instantiate
    public Transform player; // Reference to the player's transform
    public float regenerationDistance = 50f; // Distance to the end of the track for regeneration
    private GameObject currentTrack; // Reference to the currently active track
    private Transform lastCheckpoint; // Reference to the last checkpoint

    void Start()
    {
        GenerateNewTrack(Vector3.zero, Quaternion.identity);
    }

    void Update()
    {
        CheckAndRegenerateTrack();
    }

    void GenerateNewTrack(Vector3 position, Quaternion rotation )
    {
        currentTrack = Instantiate(trackPrefab, position, rotation); // Instantiate the track prefab
        UpdateLastCheckpoint();
    }

    void UpdateLastCheckpoint()
    {
        // Find all checkpoints under the currentTrack
        Transform checkpointsContainer = currentTrack.transform;

        lastCheckpoint = null;
        int highestIndex = -1;

        // Iterate through all children to find the highest-indexed checkpoint
        foreach (Transform child in checkpointsContainer)
        {
            if (child.name.StartsWith("Checkpoint_"))
            {
                string indexString = child.name.Replace("Checkpoint_", "");
                if (int.TryParse(indexString, out int index))
                {
                    if (index > highestIndex)
                    {
                        highestIndex = index;
                        lastCheckpoint = child;
                    }
                }
            }
        }
    }

    void CheckAndRegenerateTrack()
    {
        if (lastCheckpoint != null)
        {
            float distanceToLastCheckpoint = Vector3.Distance(player.position, lastCheckpoint.position);

            if (distanceToLastCheckpoint < regenerationDistance)
            {
                Vector3 newTrackPosition = lastCheckpoint.position + lastCheckpoint.forward * 10f;
                GenerateNewTrack(newTrackPosition, lastCheckpoint.rotation);
            }
        }
    }
}
