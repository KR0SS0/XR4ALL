using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightsaberJoystick : LightsaberController
{
    [SerializeField] private Animator animator;
    [SerializeField] private float smoothingSpeed = 7f; // Adjust the smoothing speed as needed
    [SerializeField] private float inputThreshold = 0.05f; // Minimum magnitude of input vector to consider it as movement
    [SerializeField] private float attackThreshold = 0.20f; // Minimum magnitude of input vector to consider it as an attack
    [SerializeField] private float attackRange = 4f;

    private Vector2 inputVector;
    private Vector3 previousPosition;

    // Update is called once per frame
    void Update()
    {
        inputVector = Gamepad.current.leftStick.ReadValue();

        // Check if input magnitude is above the threshold
        if (inputVector.magnitude > inputThreshold)
        {
            // Normalize the input vector
            inputVector = inputVector.normalized;

            // Smoothly update the animator parameters based on joystick input
            animator.SetFloat("blendX", Mathf.Lerp(animator.GetFloat("blendX"), inputVector.x, smoothingSpeed * Time.deltaTime));
            animator.SetFloat("blendY", Mathf.Lerp(animator.GetFloat("blendY"), inputVector.y, smoothingSpeed * Time.deltaTime));

            // Calculate velocity
            Vector3 currentPosition = transform.position;
            float velocity = (currentPosition - previousPosition).magnitude / Time.deltaTime;
            previousPosition = currentPosition;
            Debug.Log(velocity);



            // Check if the velocity is above the attack threshold
            if (velocity > attackThreshold)
            {
                BaseDroneController drone = FindNearestDrone();
                if (drone != null)
                {
                    float distanceToDrone = Vector3.Distance(transform.position, drone.transform.position);
                    if (distanceToDrone <= attackRange)
                    {
                        drone.HandleHit();
                        StartTriggerVibration();
                        if (!source.isPlaying)
                        {
                            PlayStrikeSound();
                        }
                        
                        Debug.Log("Drone hit!");
                    } else
                    {
                        if (!source.isPlaying)
                        {
                            PlaySwingSound();
                        }
                        Debug.Log("Drone is out of attack range.");
                    }
                } else
                {
                    Debug.Log("No drones found.");
                }
            }
        }
    }
}
