using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class ScoreCounter : MonoBehaviourPunCallbacks
{
    public int score = 0;  // Public variable to store the score

    public void AddScore(int scoreToAdd)
    {
        if (photonView.IsMine)
        {
            // AddScore method from the ScoreExtensions
            PhotonNetwork.LocalPlayer.AddScore(scoreToAdd);
        }
    }

    // This function is called when the GameObject collides with another Collider
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the tag "checkpoint"
        if (other.CompareTag("Checkpoint"))
        {
            // Increment the score if it is me who collided with the checkpoint
            if (photonView.IsMine)
            {
                Debug.Log("Checkpoint reached");
                AddScore(1);
            }
            
        }
    }
}
