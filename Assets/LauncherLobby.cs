using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine;

namespace Com.MyCompany.MyGame
{
    public class LauncherLobby : MonoBehaviourPunCallbacks
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


        // Call this function when the Play button is clicked
        public void StartGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Master client is starting the game");
                PhotonNetwork.LoadLevel("SceneMichelle");
            }

        }

    }
}