using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessibilityManager : MonoBehaviour
{
    public static AccessibilityManager Instance { get; private set; }

    private bool isAccessibilityMode;

    private void Awake()
    {
        Instance = this;
    }


    public bool IsAccessibilityMode()
    {
        return isAccessibilityMode;
    }

    public void SetIsAccessibilityMode(bool state)
    {
        isAccessibilityMode = state;
    }
}
