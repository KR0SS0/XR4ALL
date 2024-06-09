using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightsaberJoystick : LightsaberController
{
    [SerializeField] private Animator animator;
    [SerializeField] private float smoothingSpeed = 7f; // Adjust the smoothing speed as needed
    [SerializeField] private float inputThreshold = 0.05f; // Minimum magnitude of input vector to consider it as movement

    private Vector2 inputVector;

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
            
            if(!source.isPlaying)
            {
                PlaySwingSound();
            }
        }
    }
}
