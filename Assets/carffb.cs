using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car; // Add this line to include the CarController namespace

namespace UnityFFB
{
    public class CarFFB : MonoBehaviour
    {
        private UnityFFB ffb;
        public CarController carController;  // Reference to the car controller

        // Force feedback parameters
        public float speedFactor = 1f;  // Factor to scale force based on speed
        public float steeringFactor = 1f;  // Factor to scale force based on steering angle
        public float collisionForce = 500f;  // Force applied during collisions

        private void Start()
        {
            // Find the UnityFFB instance in the scene
            ffb = UnityFFB.instance;

            if (ffb == null)
            {
                Debug.LogError("UnityFFB instance not found. Make sure the UnityFFB script is attached to a GameObject in the scene.");
                return;
            }

            // Find the CarController instance in the scene
            

            if (carController == null)
            {
                Debug.LogError("CarController instance not found. Make sure the CarController script is attached to a GameObject in the scene.");
                return;
            }

            // Start the coroutine to apply force feedback based on car behavior
            StartCoroutine(ApplyForceFeedback());
        }

        private IEnumerator ApplyForceFeedback()
        {
            while (true)
            {
                // Calculate force feedback based on car speed and steering angle
                float speed = carController.CurrentSpeed;
                float steeringAngle = carController.CurrentSteerAngle;
               // Debug.Log("current steering: "+steeringAngle);
                int forceValue = (int)(speed * speedFactor + math.abs(steeringAngle) * steeringFactor*0.01f);
                forceValue = 5;
                if (steeringAngle > 0)
                {
                    ffb.force = forceValue;
                }
                else if (steeringAngle < 0)
                {
                    ffb.force = -forceValue;
                }
                

                // Calculate the force based on the car's parameters
                

                // Apply the calculated force feedback
               
                

                // Debug log to track applied force feedback
                //Debug.Log($"Force feedback applied. Force: {forceValue}");

                // Wait for a short interval before updating force feedback again
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Apply force feedback during a collision
            ffb.force = (int)collisionForce;
            ffb.StartFFBEffects();
            Debug.Log("Collision detected. Force feedback applied.");

            // Stop force feedback after a short duration
            StartCoroutine(StopCollisionForceFeedback());
        }

        private IEnumerator StopCollisionForceFeedback()
        {
            yield return new WaitForSeconds(0.5f);
            ffb.force = 0;
            //ffb.StopFFBEffects();
            Debug.Log("Collision force feedback stopped.");
        }
    }
}
