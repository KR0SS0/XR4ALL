using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveDrone : BaseDroneController
{

    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private bool forceDestroy = false;

    private void Awake()
    {
        droneType = DroneType.Explosive;
    }

    // Start is called before the first frame update
    void Start()
    {
        requiredSpeed = 0f;
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

        if(state == StateMachine.Attacking)
        {
            SwitchState(0f, StateMachine.Destroy);
        }
    }

    protected override void HandleHit()
    {
        hp--;
        SwitchState(0f, StateMachine.Destroy);
    }
}
