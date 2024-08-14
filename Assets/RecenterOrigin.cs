using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.XR.CoreUtils;

public class RecenterOrigin : MonoBehaviour
{
    public XROrigin xrOrigin; 
    public float moveSpeed = 1.0f; 
    public float rotationSpeed = 100.0f; 

    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        // Horizontal and forward/backward movement (WASD)
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
        }

        // Vertical movement (ArrowUp and ArrowDown)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection += Vector3.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection += Vector3.down;
        }

        // Apply movement to the XR Origin
        moveDirection = xrOrigin.transform.TransformDirection(moveDirection);
        xrOrigin.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Rotation around the Y-axis (ArrowLeft and ArrowRight)
        float rotation = 0f;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotation -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotation += rotationSpeed * Time.deltaTime;
        }

        // Apply rotation to the XR Origin
        xrOrigin.transform.Rotate(0, rotation, 0);
    }
}
