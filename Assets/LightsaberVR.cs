using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LightsaberVR : LightsaberController
{
    public XRNode controllerNode;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    private InputDevice controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);
        controller.TryGetFeatureValue(CommonUsages.devicePosition, out previousPosition);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition;

        if (controller.TryGetFeatureValue(CommonUsages.devicePosition, out currentPosition))
        {
            currentVelocity = (currentPosition - previousPosition) / Time.deltaTime;
            previousPosition = currentPosition;
        }
        PlayVRSwingSound(GetVelocity());
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    public Vector3 GetSwingDirection()
    {
        return currentVelocity.normalized;
    }
}
