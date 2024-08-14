using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateA : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // rotate this transform around its self in random directions with small steps
        transform.Rotate(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        
        

        
    }
}
