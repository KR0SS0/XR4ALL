using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OneHitDrone : BaseDroneController
{
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private bool forceDestroy = false;
    public static float MaxDistanceToPlayer { get => maxDistanceToPlayer; }
    private bool isHit = false;

    private void Awake()
    {
        DroneType = DroneType.OneHit;      
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        bullet = bulletShot;
        OnStart();
    }

    private void LateUpdate()
    {
        if (forceDestroy)
        {
            SwitchState(0f, StateMachine.Destroy);
            forceDestroy = false;
        }
    }


    public override void HandleHit()
    {
        if (!isHit)
        {
            hp--;
            SwitchState(0f, StateMachine.Destroy);
            isHit = true;
        }
    }
}
