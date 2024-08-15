using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateScore : MonoBehaviourPunCallbacks
{
    public static UpdateScore Instance { get; private set; }
    public TMP_Text player1ScoreText;
    public TMP_Text player2ScoreText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        /*// Find the Canvas component
        canvas = GetComponent<Canvas>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)  // Ensure this is the local player's PhotonView
            {
                // Assign the local player's camera to the Canvas
                Camera playerCamera = Camera.main;
                canvas.worldCamera = playerCamera;
            }
        }*/
    }
}
