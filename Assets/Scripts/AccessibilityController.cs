using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class AccessibilityController : MonoBehaviour
{
    [SerializeField] private GameObject accessibilityMenu;

    [SerializeField] private TMP_Text shieldCooldown;
    [SerializeField] private TMP_Text shieldDuration;
    [SerializeField] private TMP_Text droneSpawnIntensity;

    [SerializeField] private TMP_Text sensorPreset;

    private int currentSelection = 3;
    private bool menuActive = false;

    [SerializeField] private float shieldCooldownValue = 1.0f;
    [SerializeField] private float shieldDurationValue = 2.5f;
    [SerializeField] private float voiceShieldCooldownValue = 0.5f;
    [SerializeField] private float voiceShieldDurationValue = 5.0f;
    private float droneSpawnIntensityValue = 1.0f;

    private string[] sensorPresets = { "1. Freehand", "2. Voice", "3. Joystick"};
    private int currentPresetIndex = 0;

    private void Start()
    {
        LoadSettings();
        UpdateMenu();
        accessibilityMenu.SetActive(menuActive);
        currentSelection = 3;

        Debug.Log(PlayerPrefs.GetFloat("ShieldCooldown", 1.0f) + ", " + PlayerPrefs.GetFloat("ShieldDuration", 1.0f));
    }

    private void Update()
    {
        if(GameManager.Instance.CurrentState == GameManager.GameState.WaitingToStart)
        {
            //HandleMenuNavigation();
            HandleValueChange();
            UpdateMenu();
        }
        /*
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            ToggleMenu();
        }

        if (menuActive)
        {
            HandleMenuNavigation();
            HandleValueChange();
        }
        */
    }

    private void ToggleMenu()
    {
        menuActive = !menuActive;
        accessibilityMenu.SetActive(menuActive);
    }

    private void HandleMenuNavigation()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            currentSelection = (currentSelection - 1 + 4) % 4;
            UpdateMenu();
        }

        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            currentSelection = (currentSelection + 1) % 4;
            UpdateMenu();
        }
    }

    private void HandleValueChange()
    {
        // Check for left and right arrow keys on the keyboard
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            ChangeValue(-1);
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            ChangeValue(1);
        }

        // Check for left and right directions on the gamepad D-pad
        /*
        if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.left.wasPressedThisFrame)
            {
                ChangeValue(-1);
            }

            if (Gamepad.current.dpad.right.wasPressedThisFrame)
            {
                ChangeValue(1);
            }

            // Check for left and right on the joystick (left stick)
            if (Gamepad.current.leftStick.left.wasPressedThisFrame)
            {
                ChangeValue(-1);
            }

            if (Gamepad.current.leftStick.right.wasPressedThisFrame)
            {
                ChangeValue(1);
            }
            if (Gamepad.current.rightStick.left.wasPressedThisFrame)
            {
                ChangeValue(-1);
            }

            if (Gamepad.current.rightStick.right.wasPressedThisFrame)
            {
                ChangeValue(1);
            }
        }
        */
    }


    private void ChangeValue(int direction)
    {
        switch (currentSelection)
        {
            case 0:
            shieldCooldownValue = Mathf.Clamp(shieldCooldownValue + direction * 0.1f, 0.1f, 10.0f);
            shieldCooldown.text = "Shield Cooldown: " + shieldCooldownValue.ToString("F1");
            PlayerPrefs.SetFloat("ShieldCooldown", shieldCooldownValue);
            break;
            case 1:
            shieldDurationValue = Mathf.Clamp(shieldDurationValue + direction * 0.1f, 0.1f, 20.0f);
            shieldDuration.text = "Shield Duration: " + shieldDurationValue.ToString("F1");
            PlayerPrefs.SetFloat("ShieldDuration", shieldDurationValue);
            break;
            case 2:
            droneSpawnIntensityValue = Mathf.Clamp(droneSpawnIntensityValue + direction * 0.1f, 0.1f, 5.0f);
            droneSpawnIntensity.text = "Drone Spawn Intensity: " + droneSpawnIntensityValue.ToString("F1");
            PlayerPrefs.SetFloat("DroneSpawnIntensity", droneSpawnIntensityValue);
            break;
            case 3:
            currentPresetIndex = (currentPresetIndex + direction + sensorPresets.Length) % sensorPresets.Length;
            sensorPreset.text = "Sensor Preset: " + sensorPresets[currentPresetIndex];
            PlayerPrefs.SetInt("SensorPresetIndex", currentPresetIndex);
            break;
        }
    }

    private void UpdateMenu()
    {
        shieldCooldown.color = currentSelection == 0 ? Color.yellow : Color.white;
        shieldDuration.color = currentSelection == 1 ? Color.yellow : Color.white;
        droneSpawnIntensity.color = currentSelection == 2 ? Color.yellow : Color.white;
        sensorPreset.color = currentSelection == 3 ? Color.yellow : Color.white;

        shieldCooldown.text = "Shield Cooldown: " + shieldCooldownValue.ToString("F1");
        shieldDuration.text = "Shield Duration: " + shieldDurationValue.ToString("F1");
        droneSpawnIntensity.text = "Drone Spawn Intensity: " + droneSpawnIntensityValue.ToString("F1");
        sensorPreset.text = "Controller: " + sensorPresets[currentPresetIndex];
    }

    private void LoadSettings()
    {
        //shieldCooldownValue = PlayerPrefs.GetFloat("ShieldCooldown", 1.0f);
        //shieldDurationValue = PlayerPrefs.GetFloat("ShieldDuration", 1.0f);
        //droneSpawnIntensityValue = PlayerPrefs.GetFloat("DroneSpawnIntensity", 1.0f);
        currentPresetIndex = PlayerPrefs.GetInt("SensorPresetIndex", 1);
    }

    public float GetShieldCooldownValue() => currentPresetIndex == 1 ? voiceShieldCooldownValue : shieldCooldownValue;
    public float GetShieldDurationValue() => currentPresetIndex == 1 ? voiceShieldDurationValue : shieldDurationValue;
    public float GetControllerPresetIndex() => currentPresetIndex;

    public string GetInputTypeString() => sensorPreset.text;
}
