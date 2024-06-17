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
    [SerializeField] private bool debug = false;
    [SerializeField] private Transform playerTransform;

    private Color sphereColor = Color.black;
    private Color spawnAreaColor = Color.red;
    private Mesh cylinderMesh;
    private float sphereRadius; //drone distance to player

    private float minAngle;
    private float maxAngle;

    private Transform playerLocation;
    private readonly List<GameObject> activeDrones = new List<GameObject>();
    private int currentRound = 0;
    private bool isGameOngoing = false;
    private bool isRoundFinished = true;
    private float ySpawnPosition;
    private float originalSpawnTime;

    private int currentOneHitToSpawn;
    private int currentTwoHitsToSpawn;
    private int currentExplosiveToSpawn;
    private int currentOneHitKilled;
    private int currentTwoHitsKilled;
    private int currentExplosiveKilled;
    [SerializeField] private int maxActiveDrones = 8;

    public float MinDistanceToPlayer { get => minDistanceToPlayer; set => minDistanceToPlayer = value; }
    public float MaxDistanceToPlayer { get => maxDistanceToPlayer; set => maxDistanceToPlayer = value; }
    public float SpawnAngle { get => spawnAngle; set => spawnAngle = value; }

    private void Start()
    {
        playerLocation = GameObject.FindGameObjectWithTag("Player").transform;
        ySpawnPosition = transform.position.y;
        minAngle = - spawnAngle / 2f;
        maxAngle = spawnAngle / 2f;
        originalSpawnTime = spawnTime;
        sphereRadius = BaseDroneController.MaxDistanceToPlayer;
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
        currentOneHitKilled = 0;
        currentTwoHitsKilled = 0;
        currentExplosiveKilled = 0;

        StartCoroutine(ActivateDroneFromList());
    }

    private void CalculateDroneAmount()
    {
        switch (currentRound)
        {
            case 1:
                currentOneHitToSpawn = round1OneHitCount;
                currentTwoHitsToSpawn = round1TwoHitsCount;
                currentExplosiveToSpawn = round1ExplosiveCount;
                break;
            case 2:
                currentOneHitToSpawn = round2OneHitCount;
                currentTwoHitsToSpawn = round2TwoHitsCount;
                currentExplosiveToSpawn = round2ExplosiveCount;
                break;
            case 3:
                currentOneHitToSpawn = round3OneHitCount;
                currentTwoHitsToSpawn = round3TwoHitsCount;
                currentExplosiveToSpawn = round3ExplosiveCount;
                break;
            default:
                currentOneHitToSpawn = Mathf.RoundToInt(round3OneHitCount * (1 + spawnPercentageIncrease / 100f));
                currentTwoHitsToSpawn = Mathf.RoundToInt(round3TwoHitsCount * (1 + spawnPercentageIncrease / 100f));
                currentExplosiveToSpawn = Mathf.RoundToInt(round3ExplosiveCount * (1 + spawnPercentageIncrease / 100f));
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
        newDrone.SetActive(false);
    }

    private float CalculateRandomInterval(int totalDrones)
    {
        float spawnInterval = spawnTime / totalDrones;
        float randomTime = Random.Range(0f, spawnInterval * 2f);
        spawnTime -= spawnInterval;
        return randomTime;
    }

    private IEnumerator ActivateDroneFromList()
    {
        CalculateDroneAmount();

        // Add OneHit drones
        for (int i = 0; i < currentOneHitToSpawn; i++)
        {
            SpawnDrone(oneHitDrone);
        }

        // Add TwoHits drones
        for (int i = 0; i < currentTwoHitsToSpawn; i++)
        {
            SpawnDrone(twoHitsDrone);
        }

        // Add Explosive drones
        for (int i = 0; i < currentExplosiveToSpawn; i++)
        {
            SpawnDrone(explosiveDrone);
        }

        SuffleList();

        int totalDrones = currentOneHitToSpawn + currentTwoHitsToSpawn + currentExplosiveToSpawn + 1;

        spawnTime = originalSpawnTime + totalDrones / 10;
        int dronesToSpawn = Mathf.Min(maxActiveDrones, totalDrones - 1);

        for (int i = 0; i < dronesToSpawn; i++)
        {
            yield return new WaitForSeconds(CalculateRandomInterval(totalDrones));
            SetDroneActive(i);
            totalDrones--;
        }
        yield return null;
    }

    private void SuffleList()
    {
        // Shuffle
        for (int i = 0; i < activeDrones.Count; i++)
        {
            int randomIndex = Random.Range(0, activeDrones.Count);
            GameObject temp = activeDrones[i];
            activeDrones[i] = activeDrones[randomIndex];
            activeDrones[randomIndex] = temp;
        }
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
        randomPosition.y = ySpawnPosition + Random.Range(-0.5f, 0.1f);

        return randomPosition;
    }

    private Quaternion GetRandomDirection()
    {
        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    public void OnDroneDestroyed(GameObject drone, DroneType type)
    {
        activeDrones.Remove(drone);

        switch (type)
        {
            case DroneType.OneHit:
                currentOneHitKilled++;
                break;
            case DroneType.TwoHits:
                currentTwoHitsKilled++;
                break;
            case DroneType.Explosive:
                currentExplosiveKilled++;
                break;
        }
        Debug.Log(type + $" destroyed. Remaining drones - OneHit: {currentOneHitToSpawn - currentOneHitKilled}, " +
            $"TwoHits: {currentTwoHitsToSpawn - currentTwoHitsKilled}, " +
            $"Explosive: {currentExplosiveToSpawn - currentExplosiveKilled}");

        if(!areAllDronesKilled() && activeDrones.Count > 0)
        {
            SpawnNext();
        }
    }

    private void SpawnNext()
    {
        for(int i = 0; i < activeDrones.Count; i++)
        {
            if (!activeDrones[i].activeSelf)
            {
                activeDrones[i].SetActive(true);
                return;
            }
        }
        Debug.Log("Current list count of drones: " + activeDrones.Count);
    }

    private bool areAllDronesKilled()
    {
        return currentOneHitToSpawn - currentOneHitKilled <= 0 && 
            currentTwoHitsToSpawn - currentTwoHitsKilled <= 0 && 
            currentExplosiveToSpawn - currentExplosiveKilled <= 0;
    }

    private void SetDroneActive(int index)
    {
        if(index < activeDrones.Count && index >= 0)
        {
            activeDrones[index].SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {

        if (playerTransform != null && debug)
        {
            // Draw the sphere
            Gizmos.color = sphereColor;
            Gizmos.DrawWireSphere(playerTransform.position, sphereRadius);


            // Draw the spawn area
            Gizmos.color = spawnAreaColor;
            if (cylinderMesh == null)
            {
                cylinderMesh = CreateCylinderMesh(minDistanceToPlayer, maxDistanceToPlayer, spawnAngle);
            }
            Gizmos.DrawWireMesh(cylinderMesh, playerTransform.position, Quaternion.identity, Vector3.one);
        }
    }

    private Mesh CreateCylinderMesh(float minRadius, float maxRadius, float angle)
    {
        Mesh mesh = new Mesh();

        int segments = 8;
        float angleStep = Mathf.Deg2Rad * angle / segments;

        int vertexCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 6];

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = i * angleStep - angleStep * segments / 2 + Mathf.PI / 2f;
            float cos = Mathf.Cos(currentAngle);
            float sin = Mathf.Sin(currentAngle);

            vertices[i] = new Vector3(cos * minRadius, 0, sin * minRadius); //  inner arc
            vertices[i + segments + 1] = new Vector3(cos * maxRadius, 0, sin * maxRadius); //  outer arc
        }

        for (int i = 0; i < segments; i++)
        {
            int innerBottomLeft = i;
            int outerBottomLeft = i + segments + 1;
            int innerBottomRight = (i + 1) % (segments + 1);
            int outerBottomRight = (i + 1) % (segments + 1) + segments + 1;

            // Bottom face
            triangles[i * 6] = innerBottomLeft;
            triangles[i * 6 + 1] = outerBottomRight;
            triangles[i * 6 + 2] = outerBottomLeft;

            triangles[i * 6 + 3] = innerBottomLeft;
            triangles[i * 6 + 4] = innerBottomRight;
            triangles[i * 6 + 5] = outerBottomRight;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
