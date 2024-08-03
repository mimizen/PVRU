using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    public InputActionAsset inputActionAsset; // Reference to the Input Action Asset

    private InputAction steeringAction; // Reference to the specific action

    public Vector2 steeringValueDisplay; // Value of the action



    private void OnEnable()
    {
        // Find the action map and action
        var actionMap = inputActionAsset.FindActionMap("Wheelsteering");
        steeringAction = actionMap.FindAction("Steering");

        // Enable the action map
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
        steeringValueDisplay = steeringValue;
        Debug.Log("Steering Value: " + steeringValue);
    }
}
