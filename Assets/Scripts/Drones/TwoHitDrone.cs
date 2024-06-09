using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoHitDrone : BaseDroneController
{

    [SerializeField] private GameObject bulletShot;
    private MeshCollider droneCollider;
    private SphereCollider sphereCollider;
    private int baseHP = 2;
    private float stunnedTime = 0.5f;
    private bool timerStarted = false;
    public AnimationCurve stunnedAnimation;
    [SerializeField] private bool forceDestroy = false;

    private void Awake()
    {
        DroneType = DroneType.TwoHits;
    }

    // Start is called before the first frame update
    void Start()
    {
        hp = baseHP;
        bullet = bulletShot;
        OnStart();
        droneCollider = GetComponentInChildren<MeshCollider>();
        droneCollider.enabled = false;
        GetComponentInChildren<VFX_Manager>().StunnedAnimation = stunnedAnimation;
        sphereCollider = GetComponentInChildren<SphereCollider>();
        movementSpeed = 2.5f;
        //Debug.Log(sphereCollider.name);

    }

    private void FixedUpdate()
    {
        if (forceDestroy)
        {
            HandleHit();
            forceDestroy = false;
        }
    }

    public override void HandleHit()
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
                StartCoroutine(StunDelay());
            }

        }
    }

    private IEnumerator StunDelay()
    {
        yield return new WaitForSeconds(stunnedTime);
        timerStarted = false;
    }
}
