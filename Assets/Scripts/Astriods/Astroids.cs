using UnityEngine;
using Photon.Pun;

public class Asteroid : MonoBehaviourPun, IPunObservable
{
    private Transform[] checkpoints;
    private int currentCheckpointIndex;
    public float speed = 10f; // Default speed of the asteroid
    public Vector3 offset; // Default offset from the checkpoint

    // Public variables to allow randomization of speed and offset
    public bool randomizeOffset = false;
    public Vector3 offsetMin;
    public Vector3 offsetMax;

    public bool randomizeSpeed = false;
    public float speedMin = 5f;
    public float speedMax = 15f;

    // Variables for smooth network synchronization
    private Vector3 networkedPosition;
    private Quaternion networkedRotation;

    public void Initialize(Transform[] checkpoints)
    {
        this.checkpoints = checkpoints;
        currentCheckpointIndex = checkpoints.Length - 1; // Start at the last checkpoint

        // Randomize speed if enabled
        if (randomizeSpeed)
        {
            speed = Random.Range(speedMin, speedMax);
        }

        // Randomize offset if enabled
        if (randomizeOffset)
        {
            offset = new Vector3(
                Random.Range(offsetMin.x, offsetMax.x),
                Random.Range(offsetMin.y, offsetMax.y),
                Random.Range(offsetMin.z, offsetMax.z)
            );
        }

        if (checkpoints.Length > 0)
        {
            transform.position = checkpoints[currentCheckpointIndex].position + offset;
            networkedPosition = transform.position;
            networkedRotation = transform.rotation;
        }
    }

    void Update()
    {
        if (checkpoints == null || checkpoints.Length == 0)
            return;

        if (photonView.IsMine)
        {
            MoveTowardsCheckpoint();
        }
        else
        {
            // Smoothly synchronize position and rotation across the network
            transform.position = Vector3.MoveTowards(transform.position, networkedPosition, speed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkedRotation, speed * Time.deltaTime * 10f);
        }
    }

    private void MoveTowardsCheckpoint()
    {
        // Move towards the current checkpoint
        Transform targetCheckpoint = checkpoints[currentCheckpointIndex];
        Vector3 targetPosition = targetCheckpoint.position + offset;

        // Rotate to look at the next checkpoint
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);
        }

        // Move towards the next checkpoint
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if we've reached the checkpoint
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentCheckpointIndex--;
            if (currentCheckpointIndex < 0)
            {
                PhotonNetwork.Destroy(gameObject); // Destroy the asteroid when it reaches the first checkpoint
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this object, so we send its data to others
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(currentCheckpointIndex);
        }
        else
        {
            // Network object, receive data
            networkedPosition = (Vector3)stream.ReceiveNext();
            networkedRotation = (Quaternion)stream.ReceiveNext();
            currentCheckpointIndex = (int)stream.ReceiveNext();
        }
    }
}
