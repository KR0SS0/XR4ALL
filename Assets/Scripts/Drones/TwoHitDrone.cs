using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHitDrone : BaseDroneController
{

    [SerializeField] private AudioClip destroyClipOverride;
    private int baseHP = 2;

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        hp = baseHP;
        DestroyClip = destroyClipOverride;
    }

    protected override void HandleHit()
    {
        hp--;

        if(hp <= 0)
        {
            //base.DestroyDrone();
        }

        else
        {

        }

    }
}
