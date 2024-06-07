using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private bool forceDestroy = false;

    private void Awake()
    {
        DroneType = DroneType.OneHit;       
    }

    // Start is called before the first frame update
    void Start()
    {
        DestroyClip = destroyClipOverride;
        bullet = bulletShot;
        OnStart();
    }

    private void FixedUpdate()
    {
        if (forceDestroy)
        {
            HandleHit();
            forceDestroy = false;
        }
    }


    protected override void HandleHit()
    {
        hp--;
        SwitchState(0f, StateMachine.Destroy);
    }
}
