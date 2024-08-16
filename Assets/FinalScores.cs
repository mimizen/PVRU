using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class FinalScores : MonoBehaviour
{
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.PlayerList[0])
            {
                player1ScoreText.text = $"Player 1 Final Score: {player.GetScore()}";
            }
            // Update Player 2's score if the player is the second in the list
            else if (player == PhotonNetwork.PlayerList[1])
            {
                player2ScoreText.text = $"Player 2 Final Score: {player.GetScore()}";
            }
        }
    }
}
