using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class PowerUpController : MonoBehaviour
{
    public enum PowerUpType
    {
        SpeedBoost,
        DownforceBoost
        // Add more power-up types as needed
    }

    public PowerUpType powerUpType;
    public float effectAmount;
    public float duration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))

        {

            Debug.Log("Power-up collected by player");
            CarPowerUpController powerUpController = other.GetComponentInParent<CarPowerUpController>();
            if (powerUpController != null)
            {
                ActivatePowerUp(powerUpController);
            }
        }
    }

    private void ActivatePowerUp(CarPowerUpController powerUpController)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                powerUpController.ApplySpeedBoost(effectAmount, duration);
                break;
            case PowerUpType.DownforceBoost:
                powerUpController.ApplyDownforceBoost(effectAmount, duration);
                break;
            // Handle more power-up types...
        }

        // Destroy the power-up object after activation
        Destroy(gameObject);
    }
}
