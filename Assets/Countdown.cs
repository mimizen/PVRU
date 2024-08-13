using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Countdown : MonoBehaviourPun
{
    public int countdownTime = 3;
    public GameObject countdownText;

    //[SerializeField] private GameObject playerPrefab;

    void Start()
    {
        // Start the countdown when the scene loads
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        while (countdownTime > 0)
        {
            photonView.RPC("UpdateCountdownText", RpcTarget.All, countdownTime.ToString()); // Updates countdown text across all clients
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // Final countdown at 1, then disable isKinematic
        photonView.RPC("UpdateCountdownText", RpcTarget.All, "GO!"); 

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

        if (countdownText.GetComponent<TMPro.TMP_Text>().text == "GO!")
        {
            StartCoroutine(DisableCountdown());
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

    IEnumerator DisableCountdown()
    {
        yield return new WaitForSeconds(1f);
        countdownText.SetActive(false);
    }
}
