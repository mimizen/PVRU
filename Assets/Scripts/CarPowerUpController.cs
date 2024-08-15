using System.Collections;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
public class CarPowerUpController : MonoBehaviour
{
    private CarController carController;

    private void Start()
    {
        carController = GetComponent<CarController>();
        if (carController == null)
        {
            Debug.LogError("CarController component not found on this GameObject.");
        }
    }

    public void ApplySpeedBoost(float boostAmount, float duration)
    {
        if (carController != null)
        {
            carController.m_Topspeed += boostAmount;
            StartCoroutine(RemoveSpeedBoostAfterTime(boostAmount, duration));
        }
    }

    private IEnumerator RemoveSpeedBoostAfterTime(float boostAmount, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (carController != null)
        {
            carController.m_Topspeed -= boostAmount;
        }
    }

    public void ApplyDownforceBoost(float boostAmount, float duration)
    {
        if (carController != null)
        {
            carController.m_Downforce += boostAmount;
            StartCoroutine(RemoveDownforceAfterTime(boostAmount, duration));
        }
    }
     public void ApplyStarBoost(float amount, float duration)
    {
        StartCoroutine(StarBoostCoroutine(amount, duration));
    }

    private IEnumerator StarBoostCoroutine(float amount, float duration)
    {
        // Example implementation: temporarily increase speed and make the car invincible
        //carController.MaxSpeed += amount;
        // Add invincibility logic here

        yield return new WaitForSeconds(duration);

        //carController.MaxSpeed -= amount;
        // Remove invincibility logic here
    }

    private IEnumerator RemoveDownforceAfterTime(float boostAmount, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (carController != null)
        {
            carController.m_Downforce -= boostAmount;
        }
    }

    // More methods for other power-ups...
}
}