using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_Hammer : MonoBehaviour
{
    private float currentVelocity;

    public float CurrentVelocity { get => currentVelocity;}

    // Start is called before the first frame update
    void Start()
    {
        currentVelocity = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentVelocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;      
    }
}
