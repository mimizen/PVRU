using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Vehicles.Car;

public class PowerUpController : MonoBehaviour
{
    public enum PowerUpType
    {
        SpeedBoost,
        StarBoost
        // Add more power-up types as needed
    }

    public PowerUpType powerUpType;
    public float effectAmount;
    public float duration;
    public GameObject explosionEffectPrefab; // Reference to the explosion particle effect

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Power-up collected by player");

            // Check if the player object is controlled by the local player
            PhotonView playerPhotonView = other.GetComponentInParent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                CarPowerUpController powerUpController = other.GetComponentInParent<CarPowerUpController>();
                if (powerUpController != null)
                {
                    ActivatePowerUp(powerUpController);
                }
            }

            // Spawn the explosion effect at the power-up's position
            if (explosionEffectPrefab != null)
            {
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            }

            // Destroy the power-up object regardless of who collected it
            Destroy(gameObject);
        }
    }

    private void ActivatePowerUp(CarPowerUpController powerUpController)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                powerUpController.ApplySpeedBoost(effectAmount, duration);
                break;
            case PowerUpType.StarBoost:
                powerUpController.ApplyStarBoost(effectAmount, duration);
                break;
            // Handle more power-up types...
        }
    }
}
