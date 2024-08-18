using System.Collections;
using UnityEditor.UI;
using UnityEngine;
namespace UnityStandardAssets.Vehicles.Car
{
    public class CarPowerUpController : MonoBehaviour
    {
        private CarController carController;
        private Collider carCollider;

        private CarUserControl carUserControl;

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
                /*carUserControl = FindAnyObjectByType<CarUserControl>();
                carUserControl.speed += 2;
                carController.m_Topspeed += boostAmount;
                
                StartCoroutine(RemoveSpeedBoostAfterTime(boostAmount, duration));*/

                this.GetComponent<Rigidbody>().AddForce(transform.forward * boostAmount,ForceMode.Impulse);
            }
        }

        private IEnumerator RemoveSpeedBoostAfterTime(float boostAmount, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (carController != null)
            {
                carController.m_Topspeed -= boostAmount;
                carUserControl = FindAnyObjectByType<CarUserControl>();
                carUserControl.speed -= 2;
            }
        }


        public void ApplySpeedDump(float boostAmount, float duration)
        {
            if (carController != null)
            {
                carUserControl = FindAnyObjectByType<CarUserControl>();
                carUserControl.speed += 0;
                carController.m_Topspeed += boostAmount;

                StartCoroutine(RemoveSpeedDumpAfterTime(boostAmount, duration));
            }
        }

        private IEnumerator RemoveSpeedDumpAfterTime(float boostAmount, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (carController != null)
            {
                carController.m_Topspeed -= boostAmount;
                carUserControl = FindAnyObjectByType<CarUserControl>();
                carUserControl.speed -= 0;
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
           // WaypointFollower waypointFollower = GetComponent<WaypointFollower>();
           // Rigidbody rigidbody = GetComponent<Rigidbody>();
          //  rigidbody.isKinematic = true;
            // put me up in the air
           // rigidbody.AddForce(Vector3.up * 1000, ForceMode.Impulse);
           // waypointFollower.turnSpeed += 5;
           // waypointFollower.push += 10;
           // StartCoroutine(StarBoostCoroutine(amount, duration));
        }

        private IEnumerator StarBoostCoroutine(float amount, float duration)
        {
             yield return new WaitForSeconds(duration);
            // Temporarily increase speed
           // WaypointFollower waypointFollower = GetComponent<WaypointFollower>();
           // Rigidbody rigidbody = GetComponent<Rigidbody>();
           // rigidbody.isKinematic = false;
            // put me up in the air
           // rigidbody.AddForce(Vector3.up * 1000, ForceMode.Impulse);
          //  waypointFollower.turnSpeed -= 5;
          //  waypointFollower.push = 0;
            

            // Disable the car's collider to make it invincible
            //if (carCollider != null)
            //{
           //     carCollider.enabled = false;
            //}



            // Wait for the duration of the power-up
           /// yield return new WaitForSeconds(duration);




            // Re-enable the car's collider
           // if (carCollider != null)
//{
             //   carCollider.enabled = true;
           // }



            // Revert the speed boost
           // if (carController != null)
           // {
                carController.m_Topspeed -= amount;
           // }
        }

        // More methods for other power-ups...
    }
}
