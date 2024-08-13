using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Serializable Fields

        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
        /// </summary>
        string gameVersion = "1";

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        /*void Start()
        {
            Connect();
        }*/

        #endregion

        void Update()
        {
            // Check if the space key is pressed and if the player is the Master Client
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
        }

        #region Public Methods

        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - if not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        /* public void Connect()
        {
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined Room - Waiting for players...");
            // Additional code can be added here to handle player UI in the lobby
        } */

        // Call this function when the Play button is clicked
        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Master client is starting the game");
                PhotonNetwork.LoadLevel("SceneMichelle"); 
            }

        }

        #endregion

    }
}