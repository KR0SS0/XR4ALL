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

    private int currentSelection = 0;
    private bool menuActive = false;

    private float shieldCooldownValue = 1.0f;
    private float shieldDurationValue = 1.0f;
    private float droneSpawnIntensityValue = 1.0f;

    private string[] sensorPresets = { "OculusDefault", "AdaptiveControllerTwoButtonLayout", "AdaptiveControllerOneButtonLayout" };
    private int currentPresetIndex = 1;

    private void Start()
    {
        LoadSettings();
        UpdateMenu();
        accessibilityMenu.SetActive(menuActive);
    }

    private void Update()
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            ToggleMenu();
        }

        if (menuActive)
        {
            HandleMenuNavigation();
            HandleValueChange();
        }
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
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            ChangeValue(-1);
        }

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            ChangeValue(1);
        }
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
        sensorPreset.text = "Sensor Preset: " + sensorPresets[currentPresetIndex];
    }

    private void LoadSettings()
    {
        shieldCooldownValue = PlayerPrefs.GetFloat("ShieldCooldown", 1.0f);
        shieldDurationValue = PlayerPrefs.GetFloat("ShieldDuration", 1.0f);
        droneSpawnIntensityValue = PlayerPrefs.GetFloat("DroneSpawnIntensity", 1.0f);
        currentPresetIndex = PlayerPrefs.GetInt("SensorPresetIndex", 1);
    }

    public float GetShieldCooldownValue() => shieldCooldownValue;
    public float GetShieldDurationValue() => shieldDurationValue;
    public float GetControllerPresetIndex() => currentPresetIndex;
}
