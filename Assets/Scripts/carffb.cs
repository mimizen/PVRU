using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace UnityFFB
{
    public class CarFFB : MonoBehaviour
    {
        private UnityFFB ffb;
        public CarController carController;

        // Force feedback parameters
        public float speedFactor = 1f;
        public float steeringFactor = 1f;
        public float playerCollisionForce = 700f;  // Force applied during collisions with another player
        public float wallCollisionForce = 500f;  // Force applied during collisions with a wall
        public float tractionForceFactor = 2f;  // Factor to increase force if the car is not slipping
        public float slipThreshold = 0.2f;  // Threshold below which the car is considered not slipping

        private void Start()
        {
            ffb = UnityFFB.instance;

            if (ffb == null)
            {
                Debug.LogError("UnityFFB instance not found. Make sure the UnityFFB script is attached to a GameObject in the scene.");
                return;
            }

            if (carController == null)
            {
                Debug.LogError("CarController instance not found. Make sure the CarController script is attached to a GameObject in the scene.");
                return;
            }

            StartCoroutine(ApplyForceFeedback());
        }

        private IEnumerator ApplyForceFeedback()
        {
            while (true)
            {
                float speed = carController.CurrentSpeed;
                float steeringAngle = carController.CurrentSteerAngle;
                float slipAmount = 0;//carController.CurrentSlipAmount;  // Assuming you have this value available in CarController

                int forceValue = (int)(speed * speedFactor + math.abs(steeringAngle) * steeringFactor);

                // Progressive force feedback based on steering angle
                if (math.abs(steeringAngle) > 0)
                {
                    forceValue += (int)(math.abs(steeringAngle) * steeringFactor * 0.1f);
                }

                // Apply extra force if the car is not slipping
                if (slipAmount < slipThreshold)
                {
                    forceValue = (int)(forceValue * tractionForceFactor);
                }

                // Apply force to the steering wheel
                if (steeringAngle > 0)
                {
                    ffb.force = forceValue;
                }
                else if (steeringAngle < 0)
                {
                    ffb.force = -forceValue;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Apply different force feedback based on the tag of the object collided with
            if (collision.gameObject.CompareTag("Player"))
            {
                ffb.force = (int)playerCollisionForce;
                Debug.Log("Collision with Player detected. Player force feedback applied.");
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                ffb.force = (int)wallCollisionForce;
                Debug.Log("Collision with Wall detected. Wall force feedback applied.");
            }
            else
            {
                ffb.force = (int)(wallCollisionForce * 0.5f); // Default force for other collisions
                Debug.Log("Collision with other object detected. Default force feedback applied.");
            }

            ffb.StartFFBEffects();
            StartCoroutine(StopCollisionForceFeedback());
        }

        private IEnumerator StopCollisionForceFeedback()
        {
            yield return new WaitForSeconds(0.25f);
            ffb.force = 0;
            Debug.Log("Collision force feedback stopped.");
        }
    }
}
