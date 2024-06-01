using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        DestroyClip = destroyClipOverride;
        playerTransform = player;
    }


    protected override void HandleHit()
    {
        //base.DestroyDrone();
    }
}
