using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonMenager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab1;  // First player prefab
    public GameObject playerPrefab2;  // Second player prefab
    public GameObject mapPrefab;      // Map prefab

    // Define spawn points
    public Transform mapSpawnPoint;   // Map spawn point
    public Transform player1SpawnPoint; // Player 1 spawn point
    public Transform player2SpawnPoint; // Player 2 spawn point

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connect to Photon master server
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        base.OnConnectedToMaster();

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom("Room1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");

        if (PhotonNetwork.IsMasterClient)
        {
            // Instantiate the map only once, by the master client, at the map spawn point
            PhotonNetwork.Instantiate(mapPrefab.name, mapSpawnPoint.position, mapSpawnPoint.rotation);
        }

        // Determine the spawn point based on the number of players in the room
        Vector3 spawnPosition;
        Quaternion spawnRotation;
        GameObject playerPrefab;

        if (PhotonNetwork.PlayerList.Length == 1)
        {
            spawnPosition = player1SpawnPoint.position;
            spawnRotation = player1SpawnPoint.rotation;
            playerPrefab = playerPrefab1;
        }
        else
        {
            spawnPosition = player2SpawnPoint.position;
            spawnRotation = player2SpawnPoint.rotation;
            playerPrefab = playerPrefab2;
        }

        // Instantiate the player at the designated spawn point
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
