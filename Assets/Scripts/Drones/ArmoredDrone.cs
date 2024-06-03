using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;

    private void Awake()
    {
        droneType = DroneType.Directional;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        DestroyClip = destroyClipOverride;
        bullet = bulletShot;
    }

    protected override void HandleHit()
    {
        throw new System.NotImplementedException();
    }

}
