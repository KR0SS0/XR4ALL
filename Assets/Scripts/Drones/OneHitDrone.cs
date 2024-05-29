using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;

    public RequiredSwingDirection direction;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        hp = 1;
        DestroyClip = destroyClipOverride;
        requiredDirection = direction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void HandleHit()
    {
        base.DestroyDrone();
    }
}
