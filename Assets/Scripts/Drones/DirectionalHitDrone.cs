using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalHitDrone : BaseDroneController
{
    public RequiredSwingDirection direction;
    [SerializeField] private GameObject bulletShot;

    private void Awake()
    {
        DroneType = DroneType.Directional;       
    }

    // Start is called before the first frame update
    void Start()
    {
        OnStart();
        requiredDirection = direction;
        bullet = bulletShot;

    }

    protected override void HandleHit()
    {
        throw new System.NotImplementedException();
    }
}
