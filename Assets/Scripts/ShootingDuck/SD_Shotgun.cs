using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Shotgun : MonoBehaviour
{

    private const string duckTag = "SD_Duck";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    // Update is called once per frame
    void FixedUpdate()
    {
        ShootRaycast();
    }

    private void ShootRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {          
            if (hit.collider.CompareTag(duckTag))
            {
                // Duck found
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                Debug.Log("Hit a duck!");
            }

            else
            {
                // No duck
                Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
                Debug.Log("No duck");
            }
        }   
        
        else
        {
            // No duck
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
            Debug.Log("No duck");
        }
    }
}
