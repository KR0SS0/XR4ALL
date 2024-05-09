using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HS_Hammer : MonoBehaviour
{
    private float currentVelocity;
    private Transform startTransform;
    public float CurrentVelocity { get => currentVelocity;}

    // Start is called before the first frame update
    void Start()
    {
        currentVelocity = 0f;
        startTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentVelocity = gameObject.GetComponent<Rigidbody>().velocity.magnitude;      
    }

    public void OnRelease()
    {
        Debug.Log("Release");
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();      
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;
        //rb.isKinematic = false;
    }
}
