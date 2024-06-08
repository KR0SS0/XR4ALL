using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class PlayerItemSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    [SerializeField] private float scaleDuration = 0.5f;

    private Vector3 shieldOriginalScale;

    [Header("Audio")]
    [SerializeField] private AudioSource shieldSource;
    [SerializeField] private AudioClip[] shieldOnClip;
    [SerializeField] private AudioClip[] shieldOffClip;
    [SerializeField] private AudioClip[] shieldBlockClip;

    [Header("UI")]
    [SerializeField] private Slider shieldCooldownSlider; // Reference to the slider
    [SerializeField] private Image shieldCooldownFillImage; // Reference to the slider's fill image
    private Color originalColor;
    [SerializeField] private Color cooldownColor;

    private AccessibilityController accessibilityController;
    private bool isShieldActive = false;
    private bool isCooldownActive = false;

    private InputAction anyInputAction;

    void Start()
    {
        shieldOriginalScale = shield.transform.localScale;
        shield.transform.localScale = Vector3.zero;
        shield.SetActive(false);

        anyInputAction = new InputAction(type: InputActionType.Button);
        anyInputAction.AddBinding("<Keyboard>/space");
        anyInputAction.AddBinding("<XRController>/triggerPressed");
        anyInputAction.AddBinding("<XRController>/gripPressed");
        anyInputAction.AddBinding("<XRController>/primaryButton");
        anyInputAction.AddBinding("<XRController>/secondaryButton");
        anyInputAction.AddBinding("<Gamepad>/buttonSouth");
        anyInputAction.performed += ctx => OnAnyInput();

        accessibilityController = FindObjectOfType<AccessibilityController>();

        // Initialize the slider with the max value being the greater of shield duration and cooldown
        float maxShieldValue = Mathf.Max(accessibilityController.GetShieldDurationValue(), accessibilityController.GetShieldCooldownValue());
        shieldCooldownSlider.maxValue = maxShieldValue;
        shieldCooldownSlider.value = maxShieldValue;
        originalColor = shieldCooldownFillImage.color;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Active)
        {
            shieldCooldownSlider.gameObject.SetActive(false);
            return;
        } else
        {
            shieldCooldownSlider.gameObject.SetActive(true);
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isShieldActive && !isCooldownActive)
        {
            ActivateShield();
        }
    }

    private void ActivateShield()
    {
        StartCoroutine(ShieldRoutine());
    }

    private IEnumerator ShieldRoutine()
    {
        isShieldActive = true;

        // Activate shield with scaling up
        yield return StartCoroutine(ScaleAndSetActive(shield, true));
        PlayShieldOnSound();

        // Change slider color to indicate shield is active
        shieldCooldownFillImage.color = originalColor;

        float shieldDuration = accessibilityController.GetShieldDurationValue();
        for (float t = 0; t < shieldDuration; t += Time.deltaTime)
        {
            shieldCooldownSlider.value = Mathf.Clamp(shieldDuration - t, 0, shieldCooldownSlider.maxValue);
            yield return null;
        }

        PlayShieldOffSound();
        yield return StartCoroutine(ScaleAndSetActive(shield, false));

        isShieldActive = false;
        StartCoroutine(ShieldCooldown());
    }

    private IEnumerator ShieldCooldown()
    {
        isCooldownActive = true;

        // Change slider color to indicate cooldown
        shieldCooldownFillImage.color = cooldownColor;

        float cooldownDuration = accessibilityController.GetShieldCooldownValue();
        for (float t = 0; t < cooldownDuration; t += Time.deltaTime)
        {
            shieldCooldownSlider.value = Mathf.Clamp(t, 0, shieldCooldownSlider.maxValue);
            yield return null;
        }

        isCooldownActive = false;

        // Reset slider color and value when cooldown is complete
        shieldCooldownFillImage.color = originalColor;
        shieldCooldownSlider.value = shieldCooldownSlider.maxValue;
    }

    private IEnumerator ScaleAndSetActive(GameObject item, bool isActive)
    {
        Vector3 initialScale = item.transform.localScale;
        Vector3 targetScale = isActive ? shieldOriginalScale : Vector3.zero;
        float elapsedTime = 0f;

        if (isActive)
        {
            item.SetActive(true);
        }

        while (elapsedTime < scaleDuration)
        {
            item.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        item.transform.localScale = targetScale;

        if (!isActive)
        {
            item.SetActive(false);
        }
    }

    private void PlayShieldOnSound()
    {
        if (shieldOnClip.Length > 0)
        {
            AudioClip clip = shieldOnClip[Random.Range(0, shieldOnClip.Length)];
            shieldSource.PlayOneShot(clip);
        }
    }

    private void PlayShieldOffSound()
    {
        if (shieldOffClip.Length > 0)
        {
            AudioClip clip = shieldOffClip[Random.Range(0, shieldOffClip.Length)];
            shieldSource.PlayOneShot(clip);
        }
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }

    public void PlayShieldBlockSound()
    {
        if (shieldBlockClip.Length > 0)
        {
            AudioClip clip = shieldBlockClip[Random.Range(0, shieldBlockClip.Length)];
            shieldSource.PlayOneShot(clip);
        }
    }

    private void OnAnyInput()
    {
        if (!isShieldActive && !isCooldownActive)
        {
            ActivateShield();
        }
    }
}
