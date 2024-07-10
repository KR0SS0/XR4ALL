using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface ITutorial
{
    public void SetManager(TutorialManager manager);

    public void Test() { Debug.Log("Test"); }

    public void HandleFailedHit();

}
