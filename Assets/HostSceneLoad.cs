using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HostSceneLoad : MonoBehaviour
{
    private static GameObject sampleInstance;

    private InputAction alpha1InputAction;
    private InputAction alpha2InputAction;
    private InputAction alpha3InputAction;

    private void Awake()
    {
        if (sampleInstance != null)
            Destroy(sampleInstance);

        sampleInstance = gameObject;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        alpha1InputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/1");
        alpha2InputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/2");
        alpha3InputAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/3");

        alpha1InputAction.performed += ctx => OnAlpha1Input();
        alpha1InputAction.Enable();
        alpha2InputAction.performed += ctx => OnAlpha2Input();
        alpha2InputAction.Enable();
        alpha3InputAction.performed += ctx => OnAlpha3Input();
        alpha3InputAction.Enable();
    }

    private void OnAlpha1Input()
    {
        SceneManager.LoadScene(0);
    }

    private void OnAlpha2Input()
    {
        SceneManager.LoadScene(1);
    }

    private void OnAlpha3Input()
    {
        SceneManager.LoadScene(2);
    }


}
