using UnityEngine;
using UnityEngine.XR;

public class LightsaberController : MonoBehaviour
{
    public XRNode controllerNode;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;

    private InputDevice controller;

    void Start()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerNode);
        controller.TryGetFeatureValue(CommonUsages.devicePosition, out previousPosition);
    }

    void Update()
    {
        Vector3 currentPosition;

        if (controller.TryGetFeatureValue(CommonUsages.devicePosition, out currentPosition))
        {
            currentVelocity = (currentPosition - previousPosition) / Time.deltaTime;
            previousPosition = currentPosition;
        }
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    public Vector3 GetSwingDirection()
    {
        // Normalize the velocity to get the direction
        return currentVelocity.normalized;
    }
}
