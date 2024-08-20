using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class LightsaberController : MonoBehaviour
{
    [Header("Audio")]
    public float requiredSpeed = 1.0f;
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;
    public float replayThreshold = 0.6f;
    public AudioClip[] swingClips;
    public AudioClip[] strikeClips;
    public AudioSource source;

    [Header("Vibration")]
    public float vibrationStrength = 0.3f;

    public void PlaySwingSound()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;

        AudioClip clip = swingClips[Random.Range(0, swingClips.Length)];
        source.clip = clip;
        source.PlayOneShot(clip);
    }

    public void PlayStrikeSound()
    {
        float randomPitch = Random.Range(minPitch, maxPitch);
        source.pitch = randomPitch;

        AudioClip clip = strikeClips[Random.Range(0, strikeClips.Length)];
        source.clip = clip;
        source.PlayOneShot(clip);
    }

    public BaseDroneController FindNearestDrone()
    {
        /*
        BaseDroneController[] drones = FindObjectsOfType<BaseDroneController>();
        BaseDroneController nearestDrone = null;
        float nearestDistance = Mathf.Infinity;

        foreach (BaseDroneController drone in drones)
        {
            float distance = Vector3.Distance(transform.position, drone.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestDrone = drone;
            }
        }

        return nearestDrone;
        */

        DroneSpawner droneSpawner = FindObjectOfType<DroneSpawner>();
        BaseDroneController drone = droneSpawner.GetClosestActiveDrone();
        if(drone != null)
        {
            return drone;
        }
        else { return null; }

    }

    [ContextMenu("Vibrate")]
    public void StartTriggerVibration() { 
        StartCoroutine(TriggerVibration());
    }
    private IEnumerator TriggerVibration()
    {
        // Check if a gamepad is connected
        if (Gamepad.current != null)
        {
            Gamepad gamepad = Gamepad.current;
            gamepad.SetMotorSpeeds(vibrationStrength, vibrationStrength); // Set light vibration
            yield return new WaitForSeconds(0.1f); // Vibration duration
            gamepad.SetMotorSpeeds(0, 0); // Stop vibration
        }

        // Check if XR controllers are connected
        var rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if(rightHandDevice != null)
        {
            if (rightHandDevice.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
            {
                uint channel = 0;
                rightHandDevice.SendHapticImpulse(channel, vibrationStrength, 0.1f);
            }
        }
        
    }
}
