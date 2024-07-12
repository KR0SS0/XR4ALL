using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequiredSwingDirection { Any, Up, Down, Left, Right }
public enum DroneType { OneHit, TwoHits, Armored, Directional, Explosive}
public enum PriorityLevel { low, medium, high }

public abstract class BaseDroneController : MonoBehaviour
{
    protected enum StateMachine { Spawn, Idle, Moving, Attacking, Destroy, Stunned, Standby}
    private enum AttackState { Braking, Charging, Attacking }
    private AttackState attackState = AttackState.Braking;
    private bool isChargingAttack = false;
    private DroneType droneType;
    protected StateMachine state;
    private StateMachine newState;
    protected RequiredSwingDirection requiredDirection;
    private PriorityLevel currentPriorityLevel = PriorityLevel.low;

    protected float requiredSpeed = 1.0f;
    private SoundManager soundManager;
    protected int hp = 1;
    private VFX_Manager vfx_Manager;
    private Rigidbody rb;
    private MeshCollider meshCollider;

    private Transform playerTransform;
    private PlayerController playerController;
    private Transform bulletSpawnLocation;
    protected GameObject bullet;
    private DroneSpawner spawner;

    private Vector3 startDirectionOffset;
    private float movementSpeed = 3f;
    private float maxMovementSpeed = 4f;
    private float maxAmplitude = 25f;
    private float minAmplitude = 5f;
    private float frequency = 1f;
    private float distanceToPlayer = float.MaxValue;
    protected static float maxDistanceToPlayer = 1.8f;
    private float yOffset = 0f;

    private float spawnAnimationTime = 2.2f;
    private float deathAnimationTime = 2.0f;
    private float chargeAttackAnimationTime = 2f;
    private float stunnedAnimationTime = 0.5f;
    private float movementAccelerationTimer = 0f;

    private float rotationTime = 2f;
    private float rotationElapsedTime = 0.0f;
    private Quaternion targetRotation;
    private Quaternion initialRotation;

    private Vector3 targetPosition; //target point that moves in a sinus wave
    private Vector3 targetLocation; //random location around player
    private float gizmoSize = 0.2f;

    private float impulseMultiplier = 1f;

    protected bool tutorialDrone = false;
    private bool staticDrone = false;

    protected void OnStart()
    {
        state = StateMachine.Spawn;
        newState = state;

        if (tutorialDrone && staticDrone)
        {
            Debug.Log("Tutorial Static Drone On Start");
            SwitchState(spawnAnimationTime, StateMachine.Standby);
        }

        else
        {
            Debug.Log("Normal Drone On Start");
            SwitchState(spawnAnimationTime, StateMachine.Idle);
        }

        rb = GetComponent<Rigidbody>();
        meshCollider = GetComponentInChildren<MeshCollider>();
        soundManager = GetComponent<SoundManager>();
        vfx_Manager = GetComponentInChildren<VFX_Manager>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerTransform = playerObject.transform;
        playerController = playerObject.transform.root.GetComponent<PlayerController>();

        float[] timers = new float[4] { spawnAnimationTime, deathAnimationTime, chargeAttackAnimationTime, stunnedAnimationTime };
        vfx_Manager.SetTimers(timers);
        vfx_Manager.PlayVFX(VFX_Type.Spawn);
        bulletSpawnLocation = vfx_Manager.GetBulletSpawnLocation();

        yOffset = Random.Range(-0.25f, 0.15f);
        //Debug.Log("offset: " + yOffset);

    }

    private void FixedUpdate()
    {
        HandleState();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out LightsaberVR lightsaber) && state != StateMachine.Destroy)  
        {
            Debug.Log("Drone found saber in trigger");
            Vector3 swingDirection = lightsaber.GetSwingDirection();
            float swingSpeed = lightsaber.GetVelocity().magnitude;

            if (IsValidSwing(swingDirection, swingSpeed))
            {
                AddImpulse(swingDirection, swingSpeed);
                soundManager.PlayHitSound();
                HandleHit();
                lightsaber.StartTriggerVibration();
            }

            else if(!IsValidSwing(swingDirection, swingSpeed) && tutorialDrone && GetComponent<ITutorial>() != null)
            {
                GetComponent<ITutorial>().HandleFailedHit();
            }
        }
    }

    private void AddImpulse(Vector3 swingDirection, float swingSpeed)
    {
        //Calculate the impulse force
        Vector3 impulseForce = swingDirection * (swingSpeed * impulseMultiplier);

        //Apply the impulse
        rb.AddForce(impulseForce, ForceMode.Impulse);
    }

    private bool IsValidSwing(Vector3 direction, float speed)
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

    private void HandleState()
    {
        if (state != newState)
        {
            state = newState;
            //Debug.Log("New State updated to: " + state);

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

                case StateMachine.Stunned:
                    OnEnterStunned();
                    break;

                case StateMachine.Standby:
                    OnEnterStandby();
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

            case StateMachine.Stunned:
                StunnedUpdate();
                break;

            case StateMachine.Standby:
                StandbyUpdate();
                break;
        }
    }


    private void SpawnUpdate()
    {

    }

    private void OnEnterIdle()
    {
        SetRotationTarget();
        
    }

    private void SetRotationTarget()
    {
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
        rotationElapsedTime += Time.fixedDeltaTime;
    }

    private void OnEnterMoving()
    {
        // Set a random start direction offset in the XZ plane
        float randomAngle = Random.Range(-30f, 30f);
        startDirectionOffset = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle));
        targetLocation = GetRandomPointAroundPlayer();
        movementAccelerationTimer = 0f;
        soundManager.StartMovingSound();
    }

    private void MovingUpdate()
    {
        if (playerTransform == null) { Debug.LogWarning("Player null"); return; }

        //Debug.Log("Moving towards player");

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized + startDirectionOffset;
        directionToPlayer.y = 0;
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float currentAmplitude = Mathf.Lerp(maxAmplitude, minAmplitude, 1f - (distanceToPlayer / 100f));

        // Sinus
        float sinValue = Mathf.Sin(Time.time * frequency) * currentAmplitude * (distanceToPlayer / 10f);
        Vector3 offset = Vector3.Cross(directionToPlayer, Vector3.up) * sinValue;
        targetPosition = targetLocation + offset + Vector3.up * yOffset;

        // rotation
        Quaternion targetRotation = Quaternion.LookRotation((playerTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime);

        // Force
        Vector3 forceDirection = (targetPosition - transform.position).normalized;
        float forceMagnitude = MovementSpeed(droneType) * Time.fixedDeltaTime * 75f;
        Vector3 newVelocity = forceDirection * forceMagnitude;

        if (newVelocity.magnitude > maxMovementSpeed)
        {
            newVelocity = newVelocity.normalized * maxMovementSpeed;
        }

        rb.velocity = newVelocity;

        /*
        if(newVelocity.magnitude > maxVelocity)
        {
            maxVelocity = newVelocity.magnitude;
            Debug.Log("Max Velocity: " + maxVelocity);
        }
        */

        if (distanceToPlayer <= maxDistanceToPlayer)
        {
            SwitchState(0f, StateMachine.Attacking);
        }

        movementAccelerationTimer += Time.fixedDeltaTime;
    }

    private void OnEnterAttack()
    {
        attackState = AttackState.Braking;
        SetRotationTarget();
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
                    ChargeAttack();
                }
                RotateTowardsPlayer();
                break;

            case AttackState.Attacking:
                SwitchState(0f, StateMachine.Idle);
                break;
        } 
    }

    private void BrakingUpdate()
    {
        //brake
        rb.velocity *= 0.9f;
        if (rb.velocity.magnitude < 0.5f)
        {
            attackState = AttackState.Charging;
        }
    }

    private void ChargeAttack()
    {
        soundManager.PlayChargeAttackSound(droneType);
        rb.velocity = Vector3.zero;
        isChargingAttack = true;
        Debug.Log("Start charging attack!");
        vfx_Manager.PlayVFX(VFX_Type.ChargeAttack);
        StartCoroutine(Attack());
    }

    protected virtual IEnumerator Attack()
    {
        yield return new WaitForSeconds(chargeAttackAnimationTime);

        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= maxDistanceToPlayer && state != StateMachine.Destroy)
        {
            GameObject bulletToSpawn = Instantiate(bullet, bulletSpawnLocation.position, bulletSpawnLocation.rotation);
            if (tutorialDrone && bulletToSpawn.GetComponent<TutorialBeamShot>())
            {
                Debug.Log("TutorialDrone was:" + tutorialDrone);
                Debug.Log("Dronetype was: " + droneType);
                bulletToSpawn.GetComponent<TutorialBeamShot>().DroneType = droneType;
                Debug.Log("Now dronetype is: " + bulletToSpawn.GetComponent<TutorialBeamShot>().DroneType);
            }
           
            soundManager.PlayAttackSound();
            Debug.Log("Pew pew!");
        }

        attackState = AttackState.Attacking;
        isChargingAttack = false;
        yield return null;
    }

    private void OnEnterDestroy() 
    {
        StopAllCoroutines();

        meshCollider.enabled = false;
        soundManager.PlayDestroySound();
        if(droneType == DroneType.Explosive)
        {
            soundManager.PlayExplosionSound();
        }

        vfx_Manager.PlayVFX(VFX_Type.Destroy);

        SwitchState(0f, StateMachine.Destroy);

        StartDestroyTimer(deathAnimationTime);
    }

    private void DestroyUpdate()
    {
        Decerlerate(0.98f);
    }

    private void Decerlerate(float acceleration)
    {
        rb.velocity *= acceleration;
        if (rb.velocity.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnEnterStunned()
    {
        soundManager.PlayStunSound();
        GetComponentInChildren<AnimatorManager>().PlayStunnedAnimation();
        vfx_Manager.PlayVFX(VFX_Type.Stunned);
        SwitchState(stunnedAnimationTime, StateMachine.Idle);
    }

    private void StunnedUpdate()
    {
        Decerlerate(0.95f);
    }

    public void ForceDestroy()
    {
        SwitchState(0f, StateMachine.Destroy);
    }

    public abstract void HandleHit();

    protected virtual void OnEnterStandby() { }
    protected virtual void StandbyUpdate() { }

    public float Velocity
    {
        get { return rb.velocity.magnitude; }
    }

    protected float SpawnAnimationTime { get => spawnAnimationTime; set => spawnAnimationTime = value; }
    public float DeathAnimationTime { get => deathAnimationTime; }
    public DroneType DroneType { get => droneType; protected set => droneType = value; }

    public float DistanceToPlayer { get => distanceToPlayer;}
    public PriorityLevel CurrentPriorityLevel { get => currentPriorityLevel; set => currentPriorityLevel = value; }
    public bool StaticDrone { get => staticDrone; set => staticDrone = value; }
    public float SpawnAnimationTime1 { get => spawnAnimationTime; set => spawnAnimationTime = value; }

    private void StartDestroyTimer(float animationTime)
    {
        StartCoroutine(DestroyTimer(animationTime));
    }

    IEnumerator DestroyTimer(float waitTime)
    {
        if (spawner != null)
        {
            spawner.OnDroneDestroyed(gameObject, DroneType);
        }

        yield return new WaitForSeconds(waitTime);

        Destroy(gameObject);
    }

    protected void SwitchState(float time, StateMachine newState)
    {
        StartCoroutine(SwitchStateTimer(time, newState));
    }

    IEnumerator SwitchStateTimer(float waitTime, StateMachine newState)
    {
        yield return new WaitForSeconds(waitTime);
        this.newState = newState;
    }

    public void SetSpawner (DroneSpawner spawner)
    {
        this.spawner = spawner;
    }

    protected void HitPlayer()
    {
        playerController.Hit();
    }

    public float MovementSpeed(DroneType droneType)
    {
        float speed = 0f;

        switch (droneType)
        {
            case DroneType.OneHit:
                speed = movementSpeed;
                break;

            case DroneType.TwoHits:
                speed = movementSpeed * 0.65f;
                break;

            case DroneType.Explosive:
                speed = movementSpeed * 0.35f;
                break;
        }

        switch (currentPriorityLevel)
        {
            case PriorityLevel.low:
                speed *= 0.8f;
                break;

            case PriorityLevel.medium:
                speed *= 1f;
                break;

            case PriorityLevel.high:
                speed *= 1.2f;
                break;
        }          

        return Mathf.Min(speed * movementAccelerationTimer, speed);
    }

    private Vector3 GetRandomPointAroundPlayer()
    {
        if(spawner == null)
        {
            spawner = FindFirstObjectByType<DroneSpawner>();
        }

        //degrees
        float sectorAngle = spawner.SpawnAngle;
        float halfSectorAngle = sectorAngle / 2f;

        //inner and outer radius
        float outerRadius = maxDistanceToPlayer;
        float innerRadius = maxDistanceToPlayer * 0.75f;

        //Generate a random angle and random radius
        float randomAngle = Random.Range(-halfSectorAngle, halfSectorAngle) + 90f;
        float randomRadius = Random.Range(innerRadius, outerRadius);

        //Convert polar coordinates to Cartesian coordinates
        float x = randomRadius * Mathf.Cos(randomAngle * Mathf.Deg2Rad);
        float z = randomRadius * Mathf.Sin(randomAngle * Mathf.Deg2Rad);

        Vector3 randomPoint = new Vector3(x, 0f, z);

        return playerTransform.position + randomPoint;
    }

    private void OnDrawGizmos()
    {
        if (targetLocation != null)
        {
            Gizmos.color = GetColor();
            Gizmos.DrawWireSphere(targetLocation, gizmoSize);
        }
    }

    private Color GetColor()
    {
        switch(droneType)
        {
            case DroneType.OneHit:
                return Color.red;
            case DroneType.TwoHits:
                return Color.blue;
            case DroneType.Explosive:
                return Color.yellow;
            default:
                return Color.white;
        }
    }

    public void SwitchLevel(PriorityLevel newLevel)
    {
        if (currentPriorityLevel == newLevel) return;

        currentPriorityLevel = newLevel;
        soundManager.SwitchLevel(currentPriorityLevel, newLevel);

    }

    public class DistanceToPlayerComparer : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            if(x == null || y == null)
            {
                return 0;
            }

            BaseDroneController xController = x.GetComponent<BaseDroneController>();
            BaseDroneController yController = y.GetComponent<BaseDroneController>();

            if (xController == null || yController == null)
            {
                return 0;
            }
            return xController.DistanceToPlayer.CompareTo(yController.DistanceToPlayer);
        }
    }
}
