using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class ScorePlaceholder : MonoBehaviourPunCallbacks
{
    private int playerScore;
    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            IncreaseScore(1);
        }
    }

    // This method can be called to increase the player's score
    public void IncreaseScore(int amount)
    {
        playerScore += amount;
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        pv.RPC("UpdateScoreUI", RpcTarget.All, playerScore, playerIndex);
    }

    [PunRPC]
    void UpdateScoreUI(int newScore, int playerIndex)
    {
        // Use the existing instance of UpdateScore to update the UI
        if (playerIndex == 0) // Player 1
        {
            UpdateScore.Instance.player1ScoreText.text = "Player 1: " + newScore.ToString();
        }
        else if (playerIndex == 1) // Player 2
        {
            UpdateScore.Instance.player2ScoreText.text = "Player 2: " + newScore.ToString();
        }
    }
}
