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
        public float steeringExponentialFactor = 1.5f;  // Exponential factor for steering force

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
                float slipAmount = 0; // carController.CurrentSlipAmount; // Assuming you have this value available in CarController

                // Calculate force value based on speed
                int baseForceValue = (int)(speed * speedFactor*0.1f);

                // Calculate force contribution from steering angle, with a softer center and exponential increase
                float steeringForce = steeringFactor * Mathf.Pow(Mathf.Abs(steeringAngle), steeringExponentialFactor);

                // Combine the forces
                int forceValue = baseForceValue + (int)steeringForce;

                // Apply extra force if the car is not slipping
                if (slipAmount < slipThreshold)
                {
                    forceValue = (int)(forceValue * tractionForceFactor);
                }

                // Determine if the steering direction is left or right relative to the car's current forward direction
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
            // Determine the point of impact relative to the car's forward direction
            Vector3 collisionDirection = collision.contacts[0].point - carController.transform.position;
            collisionDirection.Normalize();

            // Determine the angle between the car's forward direction and the collision direction
            float angle = Vector3.SignedAngle(carController.transform.forward, collisionDirection, Vector3.up);

            // Calculate the base force depending on the object collided with
            int collisionForce = 0;
            if (collision.gameObject.CompareTag("Player"))
            {
                collisionForce = (int)playerCollisionForce;
                Debug.Log("Collision with Player detected. Player force feedback applied.");
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                collisionForce = (int)wallCollisionForce;
                Debug.Log("Collision with Wall detected. Wall force feedback applied.");
            }
            else
            {
                collisionForce = (int)(wallCollisionForce * 0.5f); // Default force for other collisions
                Debug.Log("Collision with other object detected. Default force feedback applied.");
            }

            // Apply force feedback in the direction of the collision relative to the car's forward direction
            if (angle > 45f && angle <= 135f) // Right side collision
            {
                ffb.force = collisionForce;
            }
            else if (angle < -45f && angle >= -135f) // Left side collision
            {
                ffb.force = -collisionForce;
            }
            else if (angle > -45f && angle <= 45f) // Front collision
            {
                ffb.force = collisionForce;
            }
            else // Rear collision
            {
                ffb.force = -collisionForce;
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
