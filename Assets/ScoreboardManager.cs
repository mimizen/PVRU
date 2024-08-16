using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    void Update()
    {
        UpdateScoreboard();
    }

    void UpdateScoreboard()
    {
        // Iterate through all players in the room
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Update Player 1's score if the player is the first in the list
            if (player == PhotonNetwork.PlayerList[0])
            {
                player1ScoreText.text = $"Player 1: {player.GetScore()}";
            }
            // Update Player 2's score if the player is the second in the list
            else if (player == PhotonNetwork.PlayerList[1])
            {
                player2ScoreText.text = $"Player 2: {player.GetScore()}";
            }
        }
    }
}
