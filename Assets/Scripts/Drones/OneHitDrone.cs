using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private bool forceDestroy = false;
    public static float MaxDistanceToPlayer { get => maxDistanceToPlayer; }

    private void Awake()
    {
        DroneType = DroneType.OneHit;       
    }

    // Start is called before the first frame update
    void Start()
    {
        bullet = bulletShot;
        OnStart();
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
        hp--;
        SwitchState(0f, StateMachine.Destroy);
    }
}
