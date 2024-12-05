using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using System.Collections;
using System.Diagnostics;


public class GameOverLauncher : MonoBehaviour
{
    public int winningScore;
    public PhotonView photonView; // Reference to the PhotonView on another GameObject
    private bool gameOverTriggered = false; // To ensure the scene loads only once
    private float finishTime = 60;
    private float time;
    void Update()
    {

        if (!gameOverTriggered)
        {
            CheckScores();
        }
        time += Time.deltaTime;
    }

    void CheckScores()
    {
        // Iterate through all players in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Check if any player's score has reached the winning score
           /* if (player.GetScore() >= winningScore && !gameOverTriggered)
            {
                // Load the "Game Over" scene for all players
                gameOverTriggered = true; // Set the flag to prevent multiple triggers
                Debug.Log("Switching Scenes..");
                photonView.RPC("GameOver", RpcTarget.All);
                break;
            }*/
            if (finishTime <= time)
            {
                gameOverTriggered = true; // Set the flag to prevent multiple triggers
                //Debug.Log("Switching Scenes..");
                photonView.RPC("GameOver", RpcTarget.All);
                break;


            }
            
        }
    }

    [PunRPC]
    void GameOver()
    {

        PhotonNetwork.LoadLevel("GameOver");
 
    }
}
