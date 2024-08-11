using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints; 
    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Transform spawnPoint = spawnPoints[playerIndex % spawnPoints.Length];
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("New Player", spawnPoint.position, spawnPoint.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
