using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerItemSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    [SerializeField] private float scaleDuration = 0.5f;

    private GameObject currentHeldItem;
    private Vector3 swordOriginalScale;
    private Vector3 shieldOriginalScale;

    [SerializeField] private AudioSource lightsaberSource;
    [SerializeField] private AudioClip lightsaberOn;
    [SerializeField] private AudioClip lightsaberOff;
    [SerializeField] private AudioSource shieldSource;
    [SerializeField] private AudioClip shieldOn;
    [SerializeField] private AudioClip shieldOff;

    private bool isScaling = false;

    void Start()
    {
        swordOriginalScale = sword.transform.localScale;
        shieldOriginalScale = shield.transform.localScale;

        currentHeldItem = sword;
        ToggleItem(currentHeldItem, true);
        ToggleItem(shield, false);
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isScaling)
        {
            Debug.Log("Switching held item");
            ToggleItems();
        }
    }

    void ToggleItems()
    {
        // Disable and scale down the current item
        StartCoroutine(ScaleAndSetActive(currentHeldItem, false));

        // Switch to the other item
        currentHeldItem = currentHeldItem == sword ? shield : sword;

        // Enable and scale up the new item
        StartCoroutine(ScaleAndSetActive(currentHeldItem, true));
    }

    IEnumerator ScaleAndSetActive(GameObject item, bool isActive)
    {
        isScaling = true;

        Vector3 originalScale = item == sword ? swordOriginalScale : shieldOriginalScale;
        Vector3 initialScale = item.transform.localScale;
        Vector3 targetScale = isActive ? originalScale : Vector3.zero;
        float elapsedTime = 0f;

        // Play the correct sound effect
        if (isActive)
        {
            if (item == sword)
            {
                lightsaberSource.PlayOneShot(lightsaberOn);
            } else
            {
                //shieldSource.PlayOneShot(shieldOn);
            }
        } else
        {
            if (item == sword)
            {
                lightsaberSource.PlayOneShot(lightsaberOff);
            } else
            {
                //shieldSource.PlayOneShot(shieldOff);
            }
        }

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
        isScaling = false;
    }

    void ToggleItem(GameObject item, bool isActive)
    {
        if (isActive)
        {
            item.SetActive(true);
        } else
        {
            item.transform.localScale = Vector3.zero;
            item.SetActive(false);
        }
    }
}
