using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class GameManagerSpeed : MonoBehaviour {

    public CarController RR;

    public GameObject needle ;
    private float startPosition = 220f, endPosition = -41;
    private float desiredPosition;

    public float vehicleSpeed;


    // Start is called before the first frame update
    void Start()
    {
        RR = this.GetComponent<CarController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vehicleSpeed = RR.CurrentSpeed;
        updateneedle();
        
        
        
    }

    private void updateneedle(){
        desiredPosition = startPosition - endPosition;
        float temp = vehicleSpeed / 180;  //-> Value from 0-1
        needle.transform.eulerAngles = new Vector3(0,0, (startPosition - temp * desiredPosition));


    }


}

