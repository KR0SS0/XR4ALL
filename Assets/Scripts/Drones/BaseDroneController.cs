using System.Collections;
using UnityEngine;

public enum RequiredSwingDirection { Any, Up, Down, Left, Right }

public abstract class BaseDroneController : MonoBehaviour
{
    protected enum StateMachine { Spawn, Idle, Moving, Attacking, Destroy}
    private enum AttackState { Braking, Charging, Attacking }
    private AttackState attackState = AttackState.Braking;
    private bool isChargingAttack = false;

    protected StateMachine state;
    private StateMachine newState;
    protected RequiredSwingDirection requiredDirection;
    private float requiredSpeed = 1.0f;
    protected AudioClip destroyClip;
    protected AudioSource source;
    protected int hp = 1;
    protected VFX_Manager vfx_Manager;
    private Rigidbody rb;

    protected Transform playerTransform;
    private float movementSpeed = 3.5f;
    private float maxAmplitude = 25f;
    private float minAmplitude = 5f;
    private float frequency = 1f;
    private float distanceToPlayer;
    private float maxDistanceToPlayer = 2.5f;

    private float spawnAnimationTime = 4.2f;
    private float deathAnimationTime = 5.2f;

    private float rotationTime = 2f;
    private float rotationElapsedTime = 0.0f;
    private Quaternion targetRotation;
    private Quaternion initialRotation;

    protected void OnStart()
    {
        state = StateMachine.Spawn;
        newState = state;
        SwitchState(spawnAnimationTime, StateMachine.Idle);

        rb = GetComponent<Rigidbody>();

        source = GetComponent<AudioSource>();
        vfx_Manager = GetComponentInChildren<VFX_Manager>();
        vfx_Manager.PlayVFX(VFX_Type.Spawn);
    }

    private void Update()
    {
        HandleState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LightsaberController lightsaber) && state != StateMachine.Destroy)  
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
        if (state != newState)
        {
            state = newState;
            Debug.Log("New State updated to: " + state);

            switch (state)
            {
                case StateMachine.Idle:
                    OnEnterIdle();
                    break;

                case StateMachine.Moving:
                    OnEnterMoving();
                    break;

                case StateMachine.Attacking:
                    OnEnterAttack();
                    break;

                case StateMachine.Destroy:
                    OnEnterDestroy();
                    break;
            }
        }

        switch (state)
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

    private void OnEnterIdle()
    {
        Debug.Log("rotation updated");
        initialRotation = transform.rotation;
        float y = Quaternion.LookRotation(playerTransform.position - transform.position).eulerAngles.y;
        targetRotation = Quaternion.Euler(0f, y, 0f);
        rotationElapsedTime = 0;
    }

    private void IdleUpdate()
    {

        if (rotationElapsedTime < rotationTime)
        {
            RotateTowardsPlayer();
            rotationElapsedTime += Time.deltaTime;
        }
        else
        {
            SwitchState(0f, StateMachine.Moving); 
        }
    }

    private void RotateTowardsPlayer()
    {
        float t = Mathf.Clamp(rotationElapsedTime, 0f, rotationTime);
        transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
    }

    private void OnEnterMoving()
    {

    }

    private void MovingUpdate()
    {
        if (playerTransform == null) { Debug.LogWarning("Player null"); return; }

        Debug.Log("Moving towards player");

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        directionToPlayer.y = 0;
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float currentAmplitude = Mathf.Lerp(maxAmplitude, minAmplitude, 1f - (distanceToPlayer / 100f));

        // Sinus
        float sinValue = Mathf.Sin(Time.time * frequency) * currentAmplitude * (distanceToPlayer / 10f);
        Vector3 offset = Vector3.Cross(directionToPlayer, Vector3.up) * sinValue;
        Vector3 targetPosition = playerTransform.position + offset;
        targetPosition.y = transform.position.y; 

        // Force
        Vector3 forceDirection = (targetPosition - transform.position).normalized;
        float forceMagnitude = movementSpeed * Time.deltaTime * 75f;
        Vector3 newVelocity = forceDirection * forceMagnitude;
        newVelocity.y = rb.velocity.y; 
        rb.velocity = newVelocity;

        if (distanceToPlayer <= maxDistanceToPlayer)
        {
            SwitchState(0f, StateMachine.Attacking);
        }
    }

    private void OnEnterAttack()
    {
        attackState = AttackState.Braking;

    }

    private void AttackingUpdate()
    {
        switch (attackState)
        {
            case AttackState.Braking:
                BrakingUpdate();
                break;

            case AttackState.Charging:
                if (!isChargingAttack)
                {
                    rb.velocity = Vector3.zero;
                    StartCoroutine(ChargeAttack());
                    isChargingAttack = true;
                }
                break;

            case AttackState.Attacking:
                SwitchState(0f, StateMachine.Idle);
                break;
        } 
    }

    private void BrakingUpdate()
    {
        //brake
        rb.velocity *= 0.98f;
        if (rb.velocity.magnitude < 0.1f)
        {
            attackState = AttackState.Charging;
        }
    }

    private IEnumerator ChargeAttack()
    {
        Debug.Log("Start charging attack!");
        yield return new WaitForSeconds(2f);
        Debug.Log("Pew pew!");
        isChargingAttack = false;
        attackState = AttackState.Attacking;
    }

    protected void OnEnterDestroy() 
    {
        StopAllCoroutines();

        source.PlayOneShot(destroyClip);

        if (!vfx_Manager.IsParticleSystemDone)
        {
            vfx_Manager.InterruptParticleSystems();
        }

        vfx_Manager.PlayVFX(VFX_Type.Destroy);

        SwitchState(0f, StateMachine.Destroy);

        PlayDestroyAnimation(deathAnimationTime);
    }

    private void DestroyUpdate() 
    { 

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
