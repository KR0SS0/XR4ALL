using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;

    private void Awake()
    {
        droneType = DroneType.OneHit;       
    }

    // Start is called before the first frame update
    void Start()
    {
        DestroyClip = destroyClipOverride;
        bullet = bulletShot;
        OnStart();
    }


    protected override void HandleHit()
    {
        hp--;
        SwitchState(0f, StateMachine.Destroy);
    }
}
