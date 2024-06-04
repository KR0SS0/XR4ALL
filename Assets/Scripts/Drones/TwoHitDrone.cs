using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHitDrone : BaseDroneController
{

    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;
    private MeshCollider droneCollider;
    private SphereCollider sphereCollider;
    private int baseHP = 2;
    private float stunnedTime = 0.5f;
    private float timer = 0f;
    private bool timerStarted = false;
    public AnimationCurve stunnedAnimation;

    private void Awake()
    {
        droneType = DroneType.TwoHits;
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = baseHP;
        DestroyClip = destroyClipOverride;
        bullet = bulletShot;
        OnStart();
        droneCollider = GetComponentInChildren<MeshCollider>();
        droneCollider.enabled = false;
        GetComponentInChildren<VFX_Manager>().StunnedAnimation = stunnedAnimation;
        sphereCollider = GetComponentInChildren<SphereCollider>();

    }

    private void FixedUpdate()
    {
        if (timerStarted)
        {
            timer += Time.fixedDeltaTime;

            if(timer > stunnedTime)
            {
                timerStarted = false;
            }
        }
    }

    protected override void HandleHit()
    {
        if (!timerStarted)
        {
            hp--;

            if(hp <= 0)
            {
                timerStarted = true;
                SwitchState(0f, StateMachine.Destroy);
            }

            else
            {
                timerStarted = true;
                sphereCollider.enabled = false;
                droneCollider.enabled = true;
                SwitchState(0f, StateMachine.Stunned);
            }

        }


    }
}
