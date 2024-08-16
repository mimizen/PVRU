using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ScorePlaceholder : MonoBehaviourPunCallbacks
{
    public static ScorePlaceholder Instance; // Singleton instance

    public Text player1ScoreText;
    public Text player2ScoreText;

    private int playerScore;
    PhotonView pv;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
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
    public void UpdateScoreUI(int newScore, int playerIndex)
    {
        // Update the UI based on player index
        if (playerIndex == 0) // Player 1
        {
            if (player1ScoreText != null)
            {
                player1ScoreText.text = "Player 1: " + newScore.ToString();
            }
            else
            {
                Debug.LogWarning("Player 1 score text is not assigned in the Inspector!");
            }
        }
        else if (playerIndex == 1) // Player 2
        {
            if (player2ScoreText != null)
            {
                player2ScoreText.text = "Player 2: " + newScore.ToString();
            }
            else
            {
                Debug.LogWarning("Player 2 score text is not assigned in the Inspector!");
            }
        }
        else
        {
            Debug.LogWarning($"Unexpected player index: {playerIndex}");
        }
    }
}
