using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedDrone : BaseDroneController
{
    [SerializeField] private GameObject bulletShot;

    private void Awake()
    {
        DroneType = DroneType.Directional;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        bullet = bulletShot;
    }

    public override void HandleHit()
    {
        throw new System.NotImplementedException();
    }

}
