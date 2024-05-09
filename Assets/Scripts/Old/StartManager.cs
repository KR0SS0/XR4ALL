using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class StartManager : MonoBehaviour
{
    [SerializeField] private InputActionProperty[] accessibilityModeInputTriggers;
    [SerializeField] private InputActionProperty[] nonAssistModeInputTriggers;


    private void Update()
    {
        foreach (var action in accessibilityModeInputTriggers)
        {
            if(action.action.triggered)
            {
                AccessibilityManager.Instance.SetIsAccessibilityMode(true);
                LoadScenes.Instance.StartLoadScenes(gameObject);
                Destroy(gameObject);
            } 
        }

        foreach (var action in nonAssistModeInputTriggers)
        {
            if (action.action.triggered)
            {
                AccessibilityManager.Instance.SetIsAccessibilityMode(false);
                LoadScenes.Instance.StartLoadScenes(gameObject);
                Destroy(gameObject);
            }
        }
    }

    
}
