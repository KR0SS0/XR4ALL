using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBeamShot : BeamShot
{
    private DroneType droneType = DroneType.Directional;

    public DroneType DroneType { get => droneType; set => droneType = value; }

}
