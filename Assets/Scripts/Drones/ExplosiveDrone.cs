using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveDrone : BaseDroneController
{

    [SerializeField] private GameObject bulletShot;
    [SerializeField] private bool forceDestroy = false;
    [SerializeField] private float chargeExplosionDuration = 1.5f;

    public float ChargeExplosionDuration { get => chargeExplosionDuration;}
    public static float MaxDistanceToPlayer = 1f;

    private void Awake()
    {
        DroneType = DroneType.Explosive;
    }

    // Start is called before the first frame update
    void Start()
    {
        requiredSpeed = 0f;
        maxDistanceToPlayer = 1f;
        bullet = bulletShot;
        OnStart();
    }

    private void LateUpdate()
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
        Debug.Log("Explosion by being attacked");
        HitPlayer();

    }


    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(chargeExplosionDuration);
        SwitchState(0f, StateMachine.Destroy);
        Debug.Log("Explosion by attacking");
        HitPlayer();
        yield return null;
    }
}
