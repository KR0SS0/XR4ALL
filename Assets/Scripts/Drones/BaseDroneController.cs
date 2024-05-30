using System.Collections;
using UnityEngine;

public enum RequiredSwingDirection { Any, Up, Down, Left, Right }

public abstract class BaseDroneController : MonoBehaviour
{
    protected enum StateMachine { Spawn, Idle, Moving, Attacking, Destroy}

    private StateMachine state;
    protected StateMachine newState;
    protected RequiredSwingDirection requiredDirection;
    private float requiredSpeed = 1.0f;
    protected AudioClip destroyClip;
    protected AudioSource source;
    protected int hp = 1;
    protected VFX_Manager vfx_Manager;

    private float spawnAnimationTime = 4.2f;
    private float deathAnimationTime = 5.2f;

    protected void OnStart()
    {
        state = StateMachine.Spawn;
        newState = state;
        SwitchState(spawnAnimationTime, StateMachine.Idle);

        source = GetComponent<AudioSource>();
        vfx_Manager = GetComponentInChildren<VFX_Manager>();
        vfx_Manager.PlayVFX(VFX_Type.Spawn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightsaberController lightsaber))
        {
            Debug.Log("Drone found saber in trigger");
            Vector3 swingDirection = lightsaber.GetSwingDirection();
            float swingSpeed = lightsaber.GetVelocity().magnitude;

            if (IsValidSwing(swingDirection, swingSpeed))
            {
                HandleHit();
            }
        }
    }

    protected bool IsValidSwing(Vector3 direction, float speed)
    {
        Debug.Log("Speed: " + speed);

        if (speed < requiredSpeed)
        {
            Debug.Log("Not enough speed in swing");
            return false;
        }

        switch (requiredDirection)
        {
            case RequiredSwingDirection.Any:
                return true;
            case RequiredSwingDirection.Up:
                return Vector3.Dot(direction, Vector3.up) > 0.7f;
            case RequiredSwingDirection.Down:
                return Vector3.Dot(direction, Vector3.down) > 0.7f;
            case RequiredSwingDirection.Left:
                return Vector3.Dot(direction, Vector3.left) > 0.7f;
            case RequiredSwingDirection.Right:
                return Vector3.Dot(direction, Vector3.right) > 0.7f;
            default:
                return false;
        }
    }

    protected void HandleState()
    {
        if (state == newState) return;


        switch(state)
        {
            case StateMachine.Spawn:
                SpawnUpdate(); 
                break;

            case StateMachine.Idle:
                IdleUpdate();
                break;

            case StateMachine.Moving:
                MovingUpdate();
                break;

            case StateMachine.Attacking:
                AttackingUpdate();
                break;

            case StateMachine.Destroy:
                DestroyUpdate();
                break;
        }
    }

    private void SpawnUpdate()
    {

    }

    private void IdleUpdate()
    {

    }

    private void MovingUpdate()
    {

    }

    private void AttackingUpdate()
    {

    }

    private void DestroyUpdate() 
    { 

    }

    protected void DestroyDrone() 
    {
        Debug.Log("Destroy drone");
        source.PlayOneShot(destroyClip);

        if (!vfx_Manager.IsParticleSystemDone)
        {
            vfx_Manager.InterruptParticleSystems();
        }

        vfx_Manager.PlayVFX(VFX_Type.Destroy);

        SwitchState(0.1f, StateMachine.Destroy);

        PlayDestroyAnimation(deathAnimationTime);
    }

    protected abstract void HandleHit();

    protected AudioClip DestroyClip
    {
        get { return destroyClip; }
        set { destroyClip = value; }
    }

    protected void PlayDestroyAnimation(float animationTime)
    {
        StartCoroutine(DestroyAnimationTimer(animationTime));
    }

    IEnumerator DestroyAnimationTimer(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }

    private void SwitchState(float time, StateMachine newState)
    {
        StartCoroutine(SwitchStateTimer(time, newState));
    }

    IEnumerator SwitchStateTimer(float waitTime, StateMachine newState)
    {
        yield return new WaitForSeconds(waitTime);
        this.newState = newState;
    }
}
