using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SD_Duck : MonoBehaviour
{
    private Vector3 originalRotation;
    private Vector3 deadRotation = Vector3.forward * 90f;
    private Vector3 turningRotation;
    [SerializeField] private float rotationTime = 0.35f;
    [SerializeField] private float respawnLowBoundTime = 1.2f;
    [SerializeField] private float respawnHighBoundTime = 2.4f;

    private float rotationSpeed;
    private float elapsedTime = 0f;

    private enum DuckState { Idle, Spin, Killed, Dead, Respawning }
    private DuckState currentState = DuckState.Idle;

    [SerializeField] private float verticalFrequency = 1f;
    [SerializeField] private float verticalAmplitude = 1f;
    [SerializeField] private float horizontalFrequency = 0.2f;
    [SerializeField] private float horizontalAmplitude = 10f;
    private Vector3 startPosition;
    private float horizontalPhase;

    public float HorizontalPhase { get => horizontalPhase; set => horizontalPhase = value; }

    [SerializeField]
    private bool debug = false;
    private bool isTurning = false;

    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.rotation.eulerAngles;
        startPosition = transform.localPosition;
        turningRotation = new Vector3(originalRotation.x, originalRotation.y + 180f, originalRotation.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        MoveInSinusoidalPattern();

        switch (currentState)
        {
            case DuckState.Idle:
                Idle();
                break;
            case DuckState.Spin:
                Spin();
                break;
            case DuckState.Killed:
                RotateToDeadPosition();
                break;
            case DuckState.Dead:
                Dead();
                break;
            case DuckState.Respawning:
                RotateToOriginalPosition();
                break;
        }
    }

    private void Dead()
    {

    }

    private void Idle()
    {
        if (IsAtLeftOrRightBound())
        {
            // If at the left or right bound, start turning
            currentState = DuckState.Spin;
            isTurning = true;
        }
    }

    private void Spin()
    {
        if (isTurning)
        {

            Quaternion targetRotation = Quaternion.Euler(turningRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 90f * Time.fixedDeltaTime);
            
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isTurning = false;
                float y = turningRotation.y + 180f;
                y = Mathf.Repeat(y, 360f);
                if(debug) Debug.Log("new y: " + y);
                turningRotation =  new Vector3(turningRotation.x, y, turningRotation.z);
                currentState = DuckState.Idle; // Transition back to Idle state
            }
            
        }
    }

    private bool IsAtLeftOrRightBound()
    {
        float horizontalOffset = Mathf.Sin(Time.time * horizontalFrequency + horizontalPhase) * horizontalAmplitude;
        float leftBound = startPosition.z - horizontalAmplitude;
        float rightBound = startPosition.z + horizontalAmplitude;

        return horizontalOffset - 0.01f <= leftBound || horizontalOffset + 0.01f >= rightBound;
    }

    private float CalculateRotationSpeed()
    {
        if (elapsedTime < rotationTime / 3)
        {
            return Mathf.Log10(10f * elapsedTime + 1f) / rotationTime * 90f;
        }
        else
        {
            return 1 / rotationTime * 90f;
        }
    }

    private void RotateToDeadPosition()
    {
        elapsedTime += Time.fixedDeltaTime;
        rotationSpeed = CalculateRotationSpeed();

        Quaternion targetRotation = Quaternion.Euler(deadRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            GetComponentInParent<SD_Circuit>().StartDeadTimer(gameObject, RespawnTime());
        }
    }

    public void Respawn()
    {
        verticalFrequency = SetNewFrequency(verticalFrequency);
        horizontalFrequency = SetNewFrequency(horizontalFrequency);
        currentState = DuckState.Respawning;
    }

    private void RotateToOriginalPosition()
    {
        Quaternion targetRotation = Quaternion.Euler(originalRotation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            elapsedTime = 0f;
            currentState = DuckState.Idle;
        }
    }

    IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(RespawnTime());
        verticalFrequency = SetNewFrequency(verticalFrequency);
        horizontalFrequency = SetNewFrequency(horizontalFrequency);
        currentState = DuckState.Respawning;
    }

    public void KillDuck()
    {
        if (currentState == DuckState.Idle)
        {
              currentState = DuckState.Killed;
        }
    }

    private float RespawnTime()
    {
        return Random.Range(respawnLowBoundTime, respawnHighBoundTime);
    }

    private void MoveInSinusoidalPattern()
    {
        // vertical sine function
        float verticalOffset = Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;

        // horizontal sine function
        float horizontalOffset = Mathf.Sin(Time.time * horizontalFrequency + horizontalPhase) * horizontalAmplitude;

        // Calculate the new position of the duck
        Vector3 newPosition = new Vector3(0f, verticalOffset, horizontalOffset);

        transform.localPosition = startPosition + newPosition;
    }

    private float SetNewFrequency(float newFrequency)
    {
        return Random.Range(newFrequency * 0.8f, newFrequency * 1.5f);
    }
}
