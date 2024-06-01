using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalHitDrone : BaseDroneController
{
    public RequiredSwingDirection direction;
    [SerializeField] private AudioClip destroyClipOverride;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        requiredDirection = direction;
        DestroyClip = destroyClipOverride;

    }

    protected override void HandleHit()
    {
        throw new System.NotImplementedException();
    }
}
