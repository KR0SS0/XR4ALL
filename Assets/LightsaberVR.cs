using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LightsaberVR : LightsaberController
{
    [Header("Misc")]
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

    public void PlayVRSwingSound(Vector3 velocity)
    {
        if (source.isPlaying)
        {
            if (source.clip != null)
            {
                float playbackPosition = source.time / source.clip.length;

                if (playbackPosition < replayThreshold)
                {
                    return;
                }
            }
        }
        float swingSpeed = velocity.magnitude;
        if (swingSpeed > requiredSpeed)
        {
            PlaySwingSound();
        }
    }
}
