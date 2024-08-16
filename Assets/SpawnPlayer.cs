using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    private GameObject spawnedPlayerPrefab;

    void Start()
    {
        // Instantiate the player when the scene loads, if not already instantiated
        if (SceneManager.GetActiveScene().name == "SceneMichelle" && spawnedPlayerPrefab == null && PhotonNetwork.CurrentRoom != null ||
            SceneManager.GetActiveScene().name == "GameOver" && spawnedPlayerPrefab == null && PhotonNetwork.CurrentRoom != null)
        {
            int playerIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
            Transform spawnPoint = spawnPoints[playerIndex];
            spawnedPlayerPrefab = PhotonNetwork.Instantiate("New Player", spawnPoint.position, spawnPoint.rotation);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        int playerIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        Transform spawnPoint = spawnPoints[playerIndex];
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("New Player", spawnPoint.position, spawnPoint.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
