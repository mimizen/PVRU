using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarUserControl : MonoBehaviour
    {
        public CarController m_Car; // the car controller we want to use
        //public HingeJoint steeringwheel;
        public float maxTurnAngle = 180;
        public float speed = 1f;

        public bool Wheel;
        public bool FFB;

        public InputActionAsset inputActionAsset; // Reference to the Input Action Asset

        private InputAction steeringAction; // Reference to the specific action

        public float steeringValueDisplay; // Value of the action

        private void Start()
        {
            // Start the coroutine that delays the Awake method
            StartCoroutine(DelayedAwake());
        }

        private IEnumerator DelayedAwake()
        {
            // Wait for 5 seconds before calling the Awake method
            yield return new WaitForSeconds(5f);
            AwakeDelayed();
        }

        private void AwakeDelayed()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)  // Ensure this is the local player's PhotonView
                {
                    CarController cc = player.GetComponent<CarController>();
                    if (cc != null)
                    {
                        m_Car = cc;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = steeringValueDisplay;
            float v = speed;

            // Check if m_Car is not null before calling Move
            if (m_Car != null)
            {
#if !MOBILE_INPUT
                float handbrake = CrossPlatformInputManager.GetAxis("Jump");
                m_Car.Move(h, v, v, handbrake);
#else
        m_Car.Move(h, v, v, 0f);
#endif
            }
            else
            {
                Debug.LogWarning("m_Car is not assigned!");
            }
        }


        private void OnEnable()
        {
            // Find the action map and action
            var actionMap = inputActionAsset.FindActionMap("Wheelsteering");
            steeringAction = actionMap.FindAction("Steering");

            actionMap.Enable();

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
            // Debug.Log("steering value: " + steeringValue.x);
            //Vector3 holder = steeringwheel.transform.localEulerAngles ; 
            //holder.y = steeringValue.x * maxTurnAngle;
            //steeringwheel.transform.localEulerAngles = holder;
        }
    }
}
