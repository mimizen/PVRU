using UnityEngine;
using Photon.Pun;
using System.Collections;

public class AsteroidCollisionController : MonoBehaviourPun
{
    public GameObject explosionEffectPrefab; // The particle effect prefab for the explosion
    public float effectDuration = 2f; // Duration for which the particle effect lasts

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Check if the other object has a PhotonView and if it belongs to the local player but check in parent object
            //Debug.Log("Other: " + other + "  Parent: " + other.transform.parent);
            PhotonView playerPhotonView = other.transform.parent.parent.GetComponent<PhotonView>();
            //PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            //Debug.Log("PlayerPhotonView: " + playerPhotonView+"  IsMine: "+playerPhotonView.IsMine+"collider: "+other);

            GameObject explosionEffect = Instantiate(explosionEffectPrefab,
                other.transform.position, Quaternion.identity);
            Destroy(explosionEffect, effectDuration);

            if (playerPhotonView != null)
            {
                if (playerPhotonView.IsMine)
                {
                    // Apply the effect to the local player's instance
                    ApplyEffectToPlayer(other.gameObject);
                    Debug.Log("Astroid Hit With Me");
                }

                // Ensure that only the MasterClient handles the collision destruction and effects
                if (PhotonNetwork.IsMasterClient)

                {

                   // photonView.RPC("HandleCollisionEffects", RpcTarget.All, transform.position);
                    PhotonNetwork.Destroy(gameObject); // Destroy the asteroid across the network
                }
            }
        }
    }

    void ApplyEffectToPlayer(GameObject player)
{
    
}

IEnumerator SpinPlayerCoroutine(Rigidbody playerRigidbody)
{
    // Make the Rigidbody kinematic (disable physics interactions temporarily)
    playerRigidbody.isKinematic = true;

    // Store the current velocity to move the player during the spin
    Vector3 originalVelocity = playerRigidbody.velocity;

    // Perform the spin
    float duration = 0.5f;
    float elapsed = 0f;
    Quaternion originalRotation = playerRigidbody.rotation;
    Quaternion targetRotation = originalRotation * Quaternion.Euler(0, 360f, 0);

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Update the rotation smoothly
        playerRigidbody.rotation = Quaternion.Slerp(originalRotation, targetRotation, t);

        // Move the player based on its velocity
        playerRigidbody.MovePosition(playerRigidbody.position + originalVelocity * Time.deltaTime);

        yield return null;
    }

    // Ensure final rotation is set
    playerRigidbody.rotation = targetRotation;

    // Restore the Rigidbody to its non-kinematic state
    playerRigidbody.isKinematic = false;

    // Reapply the original velocity
    playerRigidbody.velocity = originalVelocity;
}


       

        
        

        // Optionally, you can add more effects here, such as reducing health or applying a force.
    

    [PunRPC]
    void HandleCollisionEffects(Vector3 collisionPosition)
    {
        // Instantiate the particle effect at the collision position and if not null 
        if (explosionEffectPrefab == null)
        {
            Destroy(gameObject); // Destroy the asteroid if the effect prefab is missing
            return;
        }
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, collisionPosition, Quaternion.identity);
        Destroy(explosionEffect, effectDuration); // Destroy the effect after the duration
    }
}
