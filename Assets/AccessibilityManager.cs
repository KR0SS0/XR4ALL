using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance { get; private set; }

    private bool isAccessibilityMode;
    [SerializeField] private GameObject playerNoAssist;
    [SerializeField] private GameObject playerAccessibility;
    [SerializeField] private GameObject playerMenu;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public bool IsAccessibilityMode()
    {
        return isAccessibilityMode;
    }

    public void SetIsAccessibilityMode(bool state)
    {
        playerMenu.SetActive(false);
        isAccessibilityMode = state;
        if(isAccessibilityMode)
        {
            playerAccessibility.SetActive(true);
        } else
        {
            playerNoAssist.SetActive(true);
        }

    }
}
