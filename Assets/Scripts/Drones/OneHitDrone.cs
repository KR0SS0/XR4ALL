using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        DestroyClip = destroyClipOverride;
        bullet = bulletShot;
    }


    protected override void HandleHit()
    {
        hp--;
        SwitchState(0f, StateMachine.Destroy);
    }
}
