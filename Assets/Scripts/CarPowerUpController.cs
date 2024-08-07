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