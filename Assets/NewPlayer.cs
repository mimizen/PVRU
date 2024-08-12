using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;

public class NewPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Disable the hands and head for remote players
        if (!photonView.IsMine)
        {
            // These objects should remain active for the local player, but be disabled for remote players
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);
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
