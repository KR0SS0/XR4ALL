using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldedDrone : BaseDroneController
{
    [SerializeField] private AudioClip destroyClipOverride;


    // Start is called before the first frame update
    void Start()
    {
        OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void HandleHit()
    {
        throw new System.NotImplementedException();
    }

}
