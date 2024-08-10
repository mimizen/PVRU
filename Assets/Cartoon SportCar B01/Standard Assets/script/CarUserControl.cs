using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.InputSystem;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public HingeJoint steeringwheel;
        public float maxTurnAngle = 180;
        public float speed = 1f;

        public bool Wheel;
        public bool FFB;

        
        public InputActionAsset inputActionAsset; // Reference to the Input Action Asset

        private InputAction steeringAction; // Reference to the specific action

        public float steeringValueDisplay; // Value of the action


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = steeringValueDisplay;
            //Mathf.Clamp(steeringwheel.angle / maxTurnAngle, -1, 1);
            float v = speed;
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
        private void OnEnable()
    {
        // Find the action map and action
        var actionMap = inputActionAsset.FindActionMap("Wheelsteering");
        steeringAction = actionMap.FindAction("Steering");
        


        // Subscribe to the performed event
        steeringAction.performed += OnSteering;
    }

    private void OnDisable()
    {
        // Unsubscribe from the performed event
        steeringAction.performed -= OnSteering;

        // Disable the action map
        steeringAction.actionMap.Disable();
    }

    private void OnSteering(InputAction.CallbackContext context)
    {
        // Read the value of the action
       Vector2 steeringValue = context.ReadValue<Vector2>();
        steeringValueDisplay = steeringValue.x;
      // Debug.Log("steering value: "+steeringValue.x);
        //Vector3 holder = steeringwheel.transform.localEulerAngles ; 
        //holder.y = steeringValue.x * maxTurnAngle;
        //steeringwheel.transform.localEulerAngles = holder;
    }
    }
}
