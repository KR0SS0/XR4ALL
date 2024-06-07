using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveDrone : BaseDroneController
{

    [SerializeField] private AudioClip destroyClipOverride;
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private bool forceDestroy = false;
    [SerializeField] private float chargeExplosionDuration = 1.5f;

    public float ChargeExplosionDuration { get => chargeExplosionDuration;}

    private void Awake()
    {
        DroneType = DroneType.Explosive;
    }

    // Start is called before the first frame update
    void Start()
    {
        requiredSpeed = 0f;
        DestroyClip = destroyClipOverride;
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

    protected override void HandleHit()
    {
        hp--;
        SwitchState(0f, StateMachine.Destroy);
        Debug.Log("Explosion by being attacked");
    }


    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(chargeExplosionDuration);
        SwitchState(0f, StateMachine.Destroy);
        Debug.Log("Explosion by attacking");
        yield return null;
    }
}
