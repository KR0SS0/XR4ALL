using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;

    private GameObject currentHeldItem;

    void Start()
    {
        currentHeldItem = sword;
        ToggleItem(currentHeldItem, true);
        ToggleItem(shield, false);
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Switching held item");
            ToggleItems();
        }
    }

    void ToggleItems()
    {
        // Disable and scale down the current item
        ToggleItem(currentHeldItem, false);

        // Switch to the other item
        if (currentHeldItem == sword)
        {
            currentHeldItem = shield;
        } else
        {
            currentHeldItem = sword;
        }

        // Enable and scale up the new item
        ToggleItem(currentHeldItem, true);
    }

    void ToggleItem(GameObject item, bool isActive)
    {
        item.SetActive(isActive);
    }
}
