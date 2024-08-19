using System;
using System.IO.Ports;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SerialCommunication : MonoBehaviour
{
    public string comPort = "COM3"; // Public field to set the COM port in the Unity Editor
    public CarController carController; // Public reference to the CarController script

    private SerialPort serialPort;
    private float updateInterval = 0.3f; 
    private float nextUpdateTime = 0f;

    void Start()
    {
        // Initialize the SerialPort with the specified COM port and settings
        serialPort = new SerialPort(comPort, 115200);

        // Set the write timeout to a low value to ensure quick failure if there's an issue
        serialPort.WriteTimeout = 10; 

        serialPort.Open();

        // Set small write buffer to ensure data is sent immediately
        serialPort.WriteBufferSize = 1;

        // Optionally, disable the buffering of the output stream
        serialPort.BaseStream.Flush();
    }

    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            SendSpeedValue();
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void SendSpeedValue()
    {
        if (serialPort.IsOpen && carController != null)
        {
            float currentSpeed = carController.CurrentSpeed;
            //float maxSpeed = carController.MaxSpeed;
            float maxSpeed = 100f;

            // Calculate the speed percentage and scale it to a value between 0-255
            float speedPercentage = Mathf.Clamp01(currentSpeed / maxSpeed); // Get percentage between 0 and 1
            int speedValue = Mathf.RoundToInt((speedPercentage * 255)); // Scale to 100-255 range

            // Send the speed value to the ESP device as a line of text
            serialPort.WriteLine(speedValue.ToString());

            // Flush the stream to send the data immediately
            serialPort.BaseStream.Flush();

            // Discard the output buffer to avoid overflow
            serialPort.DiscardOutBuffer();
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
