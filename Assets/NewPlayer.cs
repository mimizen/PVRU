using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;
using UnityStandardAssets.Vehicles.Car;
using UnityFFB;

public class NewPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private PhotonView photonView;

    private CarController carController;

    private CarUserControl carUserControl;

    private CarFFB carFFB;

    private CarPowerUpController carPowerUpController;

    private SerialCommunication serialCommunication;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        carController = this.GetComponentInChildren<CarController>();
        carUserControl = this.GetComponentInChildren<CarUserControl>();
        carFFB = this.GetComponentInChildren<CarFFB>();
        carPowerUpController = this.GetComponentInChildren<CarPowerUpController>();
        serialCommunication = this.GetComponentInChildren<SerialCommunication>();
        rb = this.GetComponentInChildren<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        // Disable the hands and head for remote players
        if (!photonView.IsMine)
        {
            // These objects should remain active for the local player, but be disabled for remote players
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);
            carController.enabled = false;
            carUserControl.enabled = false;
            carFFB.enabled = false;
            carPowerUpController.enabled = false;
            serialCommunication.enabled = false;
            rb.isKinematic = true;
            playerPrefab.GetComponentInChildren<Camera>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Activate and map VR components only if this is the local player's instance
            head.gameObject.SetActive(true);
            leftHand.gameObject.SetActive(true);
            rightHand.gameObject.SetActive(true);

            carController.enabled = true;
            carUserControl.enabled = true;
            carFFB.enabled = true;
            carPowerUpController.enabled = true;
            serialCommunication.enabled = true;
            //rb.isKinematic = false; if i set false my car would fall down.

            MapPosition(head, XRNode.Head);
            MapPosition(leftHand, XRNode.LeftHand);
            MapPosition(rightHand, XRNode.RightHand);
        }
        else
        {
            // Deactivate components for non-local players
            head.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
            carController.enabled = false;
            carUserControl.enabled = false;
            carFFB.enabled = false;
            carPowerUpController.enabled = false;
            serialCommunication.enabled = false;
            rb.isKinematic = true;
        }
    }

    void MapPosition(Transform target, XRNode node)
    {
        // Get the position and rotation from the XR device and apply it to the target transform
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

        target.position = position;
        target.rotation = rotation;
    }
}
