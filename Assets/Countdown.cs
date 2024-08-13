using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Countdown : MonoBehaviourPun
{
    public int countdownTime = 3;
    public GameObject countdownText; // Reference to a UI Text or TMP_Text for displaying the countdown (optional)

    //[SerializeField] private GameObject playerPrefab;

    void Start()
    {
        // Start the countdown when the scene loads
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        // Optional: Show the countdown on the screen if you have a UI element
        while (countdownTime > 0)
        {
            photonView.RPC("UpdateCountdownText", RpcTarget.All, countdownTime.ToString()); // Updates countdown text across all clients
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // Final countdown at 1, then disable isKinematic
        photonView.RPC("UpdateCountdownText", RpcTarget.All, "GO!"); // Updates the text to "GO!" on all clients

        // Disable isKinematic on all player prefabs
        photonView.RPC("EnablePlayerMovement", RpcTarget.All);
    }

    [PunRPC]
    void UpdateCountdownText(string text)
    {
        if (countdownText != null)
        {
            countdownText.GetComponent<TMPro.TMP_Text>().text = text;
        }
    }

    [PunRPC]
    public void EnablePlayerMovement()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)  // Ensure this is the local player's PhotonView
            {
                Rigidbody rb = player.GetComponentInChildren<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    Debug.Log("Zoom Zoom");
                }
            }
        }
    }
}
