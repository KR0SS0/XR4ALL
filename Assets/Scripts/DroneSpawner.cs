using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject oneHitDrone;
    [SerializeField] private GameObject twoHitsDrone;
    [SerializeField] private GameObject explosiveDrone;
    [SerializeField] private float minDistanceToPlayer = 10f;
    [SerializeField] private float maxDistanceToPlayer = 50f;
    [SerializeField] private int round1OneHitCount = 5;
    [SerializeField] private int round1TwoHitsCount = 3;
    [SerializeField] private int round1ExplosiveCount = 2;
    [SerializeField] private int round2OneHitCount = 7;
    [SerializeField] private int round2TwoHitsCount = 5;
    [SerializeField] private int round2ExplosiveCount = 3;
    [SerializeField] private int round3OneHitCount = 10;
    [SerializeField] private int round3TwoHitsCount = 7;
    [SerializeField] private int round3ExplosiveCount = 5;
    [SerializeField] private float spawnTime = 5f;
    [SerializeField] private float spawnPercentageIncrease = 10f;
    [SerializeField] private float spawnAngle = 30f;
    [SerializeField] private bool startGame = false;

    private float minAngle;
    private float maxAngle;

    private Transform playerLocation;
    private List<GameObject> activeDrones = new List<GameObject>();
    private List<DroneType> droneList;
    private int currentRound = 0;
    private bool isGameOngoing = false;
    private bool isRoundFinished = true;
    private float ySpawnPosition;
    private float originalSpawnTime;

    private int currentOneHitCount;
    private int currentTwoHitsCount;
    private int currentExplosiveCount;
    private int maxActiveDrones = 10;

    private void Start()
    {
        playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
        ySpawnPosition = transform.position.y;
        minAngle = - spawnAngle / 2f;
        maxAngle = spawnAngle / 2f;
        originalSpawnTime = spawnTime;
    }

    private void FixedUpdate()
    {
        
        if (isGameOngoing && activeDrones.Count == 0)
        {
            Debug.Log("Round finished");
            isRoundFinished = true;
        }

        if(isGameOngoing && isRoundFinished)
        {
            NextRound();
        }

        if(startGame)
        {
            StartGame();
            startGame = false;
        }

    }

    public void StartGame()
    {
        isGameOngoing = true;
        isRoundFinished = false;
        currentRound = 1;
        SpawnDrones();
    }

    private void NextRound()
    {
        isRoundFinished = false;
        currentRound++;
        SpawnDrones();
    }

    public void PauseGame()
    {
        isGameOngoing = false;
        StopAllCoroutines();
    }

    public void DestroyAllDrones()
    {
        foreach (var drone in activeDrones)
        {
            Destroy(drone);
        }
        activeDrones.Clear();
        isGameOngoing = false;
        isRoundFinished = true;
    }

    public void RestartGame()
    {
        DestroyAllDrones();
        currentRound = 1;
        StartGame();
    }

    private void SpawnDrones()
    {
        StartCoroutine(SpawnDroneFromList());
    }

    private void CalculateDroneAmount()
    {
        switch (currentRound)
        {
            case 1:
                currentOneHitCount = round1OneHitCount;
                currentTwoHitsCount = round1TwoHitsCount;
                currentExplosiveCount = round1ExplosiveCount;
                break;
            case 2:
                currentOneHitCount = round2OneHitCount;
                currentTwoHitsCount = round2TwoHitsCount;
                currentExplosiveCount = round2ExplosiveCount;
                break;
            case 3:
                currentOneHitCount = round3OneHitCount;
                currentTwoHitsCount = round3TwoHitsCount;
                currentExplosiveCount = round3ExplosiveCount;
                break;
            default:
                currentOneHitCount = Mathf.RoundToInt(round3OneHitCount * (1 + spawnPercentageIncrease / 100f));
                currentTwoHitsCount = Mathf.RoundToInt(round3TwoHitsCount * (1 + spawnPercentageIncrease / 100f));
                currentExplosiveCount = Mathf.RoundToInt(round3ExplosiveCount * (1 + spawnPercentageIncrease / 100f));
                break;
        }
    }

    private void SpawnDrone(GameObject dronePrefab)
    {
        Vector3 randomPosition = GetRandomPosition();
        Quaternion randomRotation = GetRandomDirection();
        GameObject newDrone = Instantiate(dronePrefab, randomPosition, randomRotation);
        newDrone.GetComponent<BaseDroneController>().SetSpawner(this);
        activeDrones.Add(newDrone);
    }

    private float CalculateRandomInterval(int totalDrones)
    {
        float spawnInterval = spawnTime / totalDrones;
        float randomTime = Random.Range(0f, spawnInterval * 2f);
        spawnTime -= spawnInterval;
        return randomTime;
    }

    private IEnumerator SpawnDroneFromList()
    {
        droneList = DroneList();

        int totalDrones = currentOneHitCount + currentTwoHitsCount + currentExplosiveCount + 1;

        spawnTime = originalSpawnTime + totalDrones / 10;

        int currentDroneCount = droneList.Count;
        int dronesToSpawn = Mathf.Min(maxActiveDrones, currentDroneCount);

        for(int i = 0; i < dronesToSpawn; i++)
        {
            switch (droneList[i])
            {
                case DroneType.OneHit:
                    SpawnDrone(oneHitDrone);
                    yield return new WaitForSeconds(CalculateRandomInterval(totalDrones));
                    totalDrones--;
                    break;

                case DroneType.TwoHits:
                    SpawnDrone(twoHitsDrone);
                    yield return new WaitForSeconds(CalculateRandomInterval(totalDrones));
                    totalDrones--;
                    break;

                case DroneType.Explosive:
                    SpawnDrone(explosiveDrone);
                    yield return new WaitForSeconds(CalculateRandomInterval(totalDrones));
                    totalDrones--;
                    break;

                default:
                    break;
            }
        }

        Debug.Log("from" + droneList.Count);
        droneList.RemoveRange(0, dronesToSpawn);
        Debug.Log("to: " + droneList.Count);
        yield return null;
    }

    private IEnumerator SpawnAndWait(GameObject drone, float waitTime)
    {
        SpawnDrone(drone);
        yield return new WaitForSeconds(waitTime);
    }

    private List<DroneType> DroneList()
    {

        CalculateDroneAmount();

        List<DroneType> droneList = new List<DroneType>();

        // Add OneHit drones
        for (int i = 0; i < currentOneHitCount; i++)
        {
            droneList.Add(DroneType.OneHit);
        }

        // Add TwoHits drones
        for (int i = 0; i < currentTwoHitsCount; i++)
        {
            droneList.Add(DroneType.TwoHits);
        }

        // Add Explosive drones
        for (int i = 0; i < currentExplosiveCount; i++)
        {
            droneList.Add(DroneType.Explosive);
        }

        // Shuffle
        for (int i = 0; i < droneList.Count; i++)
        {
            int randomIndex = Random.Range(0, droneList.Count);
            DroneType temp = droneList[i];
            droneList[i] = droneList[randomIndex];
            droneList[randomIndex] = temp;
        }

        return droneList;
    }

    private Vector3 GetRandomPosition()
    {
 
        float randomAngle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(minDistanceToPlayer, maxDistanceToPlayer);

        //Vector3 randomPosition = playerLocation.position + new Vector3(randomDirection.x, 0, randomDirection.y) * randomDistance;
        Vector3 forward = playerLocation.forward;
        Vector3 right = playerLocation.right;
        Vector3 randomDirection = (forward * Mathf.Cos(randomAngle)) + (right * Mathf.Sin(randomAngle));
        randomDirection.Normalize();
        Vector3 randomPosition = playerLocation.position + randomDirection * randomDistance;
        randomPosition.y = ySpawnPosition + Random.Range(-1f, 0.2f);

        return randomPosition;
    }

    private Quaternion GetRandomDirection()
    {
        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    public void OnDroneDestroyed(GameObject drone, DroneType type)
    {
        activeDrones.Remove(drone);
        Debug.Log($"Drone destroyed. Remaining drones - OneHit: {currentOneHitCount}, TwoHits: {currentTwoHitsCount}, Explosive: {currentExplosiveCount}");

        if(droneList.Count > 0)
        {
            SpawnNext();
        }

        switch (type)
        {
            case DroneType.OneHit:
                currentOneHitCount--;
                break;
            case DroneType.TwoHits:
                currentTwoHitsCount--;
                break;
            case DroneType.Explosive:
                currentExplosiveCount--;
                break;
        }
    }

    private void SpawnNext()
    {
        DroneType newDrone = droneList[0];

        switch (newDrone)
        {
            case DroneType.OneHit:
                SpawnDrone(oneHitDrone);
                break;

            case DroneType.TwoHits:
                SpawnDrone(twoHitsDrone);
                break; 

            case DroneType.Explosive:
                SpawnDrone(explosiveDrone);
                break;

        }
        droneList.Remove(0);
    }
}
