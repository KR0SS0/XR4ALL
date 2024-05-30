using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        DestroyClip = destroyClipOverride;
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
    }

    protected override void HandleHit()
    {
        base.DestroyDrone();
    }
}
