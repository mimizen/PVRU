using UnityEngine;
using Photon.Pun;

public class ScoreCounter : MonoBehaviourPunCallbacks
{
    public int score = 0;  // Public variable to store the score

    // This function is called when the GameObject collides with another Collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the tag "checkpoint"
        if (other.CompareTag("Checkpoint"))
        {
            // Increment the score if it is me who collided with the checkpoint
            if (photonView.IsMine)
            {
                score++;
                other.enabled = false;
            }
            
        }
    }
}
