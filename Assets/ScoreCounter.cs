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
            // Increment the score
            score++;

            // Disable the collider to prevent further collisions
            other.enabled = false;

            // Determine the player index based on whether this player is the master client
            int playerIndex = PhotonNetwork.IsMasterClient ? 0 : 1;

            // Update the UI with the new score using ScorePlaceholder Singleton
            ScorePlaceholder.Instance.UpdateScoreUI(score, playerIndex);
        }
    }
}
