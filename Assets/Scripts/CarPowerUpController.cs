using System.Collections;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
    public class CarPowerUpController : MonoBehaviour
    {
        private CarController carController;
        private Collider carCollider;

        private void Start()
        {
            carController = GetComponent<CarController>();
            carCollider = GetComponent<Collider>();
            if (carController == null)
            {
                Debug.LogError("CarController component not found on this GameObject.");
            }
            if (carCollider == null)
            {
                Debug.LogError("Collider component not found on this GameObject.");
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
        
        public void ApplyStarBoost(float amount, float duration)
        {
            StartCoroutine(StarBoostCoroutine(amount, duration));
        }

        private IEnumerator StarBoostCoroutine(float amount, float duration)
        {
            // Temporarily increase speed
            if (carController != null)
            {
                carController.m_Topspeed += amount;
            }

      

            // Wait for the duration of the power-up
            yield return new WaitForSeconds(duration);


            // Revert the speed boost
            if (carController != null)
            {
                carController.m_Topspeed -= amount;
            }
        }
        /*
        public void ApplyZeroGravity()
        {
            StartCoroutine(ZeroGravityCoroutine());
        }

        private IEnumerator ZeroGravityCoroutine()
        {
            // Disable gravity
            if (carRigidbody != null)
            {
                carRigidbody.useGravity = false;
            }

            // Enable waypoint following
            if (carAIControl != null)
            {
                carAIControl.enabled = true;
            }

            // Wait for 3 seconds
            yield return new WaitForSeconds(3f);

            // Re-enable gravity
            if (carRigidbody != null)
            {
                carRigidbody.useGravity = true;
            }

            // Disable waypoint following
            if (carAIControl != null)
            {
                carAIControl.enabled = false;
            }
        }
        */


    }





    // More methods for other power-ups...
}

