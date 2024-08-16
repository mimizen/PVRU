using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SteeringAnimation : MonoBehaviour
{
    public GameObject steeringWheel;          // The steering wheel object in the car model
    public GameObject steeringWheelCenter;    // The center point of the steering wheel (optional)
    public float maxTurnAngle = 180f;         // Maximum rotation angle of the steering wheel

    private CarController carController;
    private Quaternion initialRotation;       // Store the initial rotation of the steering wheel

    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponentInParent<CarController>();
        if (carController == null)
        {
            Debug.LogError("SteeringAnimation requires a CarController component in the parent.");
        }

        // Store the initial rotation of the steering wheel
        initialRotation = steeringWheel.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (carController == null) return;

        // Get the current steer angle from the car controller and normalize it
        float steerAngleNormalized = carController.CurrentSteerAngle / 20f;

        // Calculate the rotation angle for the steering wheel
        float wheelRotationAngle = -steerAngleNormalized * maxTurnAngle;

        // Apply the rotation to the steering wheel relative to its initial rotation
        steeringWheel.transform.localRotation = initialRotation * Quaternion.Euler(0, 0, -wheelRotationAngle);
    }
}
