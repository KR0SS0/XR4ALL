using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeamShot : BeamShot
{
    private DroneType droneType = DroneType.Directional;

    public DroneType DroneType { get => droneType; set => droneType = value; }

    protected override void TryHitPlayer(Collider collider)
    {
        try
        {
            Debug.Log("Tutorial Bullet, dronetype: " + droneType);
            TutorialManager tutorial = FindFirstObjectByType<TutorialManager>();
            if (tutorial != null)
            {
                tutorial.OnPlayerGetHit();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to call Hit on PlayerController: " + e.Message);
        }
    }
}
