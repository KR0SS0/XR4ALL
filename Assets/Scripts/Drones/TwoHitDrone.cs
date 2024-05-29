using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHitDrone : BaseDroneController
{

    [SerializeField] private AudioClip destroyClipOverride;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        hp = 2;
        DestroyClip = destroyClipOverride;
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void HandleHit()
    {
        hp--;

        if(hp <= 0)
        {
            base.DestroyDrone();
        }

        else
        {

        }

    }
}
