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
    public GameObject playerPrefab;
    public float delayInSeconds = 2.0f; // Delay before spawning the player

    void Start()
    {
        StartCoroutine(WaitAndSpawn(delayInSeconds));
    }

    IEnumerator WaitAndSpawn(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Check scene and room conditions before instantiating
        if (SceneManager.GetActiveScene().name == "SceneMichelle" && spawnedPlayerPrefab == null && PhotonNetwork.CurrentRoom != null)
        {
            InstantiatePlayer();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        StartCoroutine(WaitAndSpawn(delayInSeconds));
    }

    private void InstantiatePlayer()
    {
        int playerIndex = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % spawnPoints.Length;
        Transform spawnPoint = spawnPoints[playerIndex];
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("New Player Hand Tracking", spawnPoint.position, spawnPoint.rotation);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (spawnedPlayerPrefab != null)
        {
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
        }
    }
}
