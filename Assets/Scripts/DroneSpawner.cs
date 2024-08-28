using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseDroneController;

public class DroneSpawner : MonoBehaviour
{
    private enum GameState { NoGame, Spawning, NextRound}
    private GameState gameState = GameState.NoGame;

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

    private bool isTutorial = false;
    private TutorialManager tutorialManager;

    private Color sphere1Color = Color.red;
    private Color sphere2Color = Color.yellow;
    private Color spawnAreaColor = Color.black;
    private Mesh cylinderMesh;
    private float sphere1Radius = OneHitDrone.MaxDistanceToPlayer;
    private float sphere2Radius = ExplosiveDrone.MaxDistanceToPlayer;

    private float minAngle;
    private float maxAngle;
    private float gapAngle = 20;

    private Transform playerLocation;
    private readonly List<GameObject> droneList = new List<GameObject>();
    private int currentRound = 0;
    private float ySpawnPosition;
    private float originalSpawnTime;
    private float waitRoundTime = 5f;

    private int currentOneHitToSpawn;
    private int currentTwoHitsToSpawn;
    private int currentExplosiveToSpawn;
    private int currentOneHitKilled;
    private int currentTwoHitsKilled;
    private int currentExplosiveKilled;
    [SerializeField] private int maxActiveDrones = 8;

    private int maxSlotsHighPriority = 2;
    private int maxSlotsMediumPriority = 4;

    public float SpawnAngle { get => spawnAngle; set => spawnAngle = value; }

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
        
        if(gameState == GameState.NextRound)
        {
            if (isTutorial)
            {
                Debug.Log("Tutorial finished");
                EndGame();
                tutorialManager.OnSuccessTestWave();
 
            }

            else
            {
                Debug.Log("Round finished");
                NextRound();
            }
        }

        if(startGame && gameState == GameState.NoGame)
        {
            Debug.Log("Drone Spawner Start Game");
            StartGame();
            startGame = false;
        }

    }

    public void StartGame()
    {
        currentRound = 1;
        SpawnDrones(0f);
        ActivateTowers();
        StartCoroutine(SetPriority());
    }

    private void NextRound()
    {
        currentRound++;
        SpawnDrones(waitRoundTime);
    }

    public void EndGame()
    {
        DeactivateTowers();
        DestroyAllDrones();

    }

    public void DestroyAllDrones()
    {
        foreach (GameObject drone in droneList)
        {
            Destroy(drone);
        }
        DestroyAllBullets();
        droneList.Clear();
        gameState = GameState.NoGame;
    }

    private void DestroyAllBullets()
    {
        BeamShot[] bullets = FindObjectsOfType<BeamShot>();

        Debug.Log("Destroying: " + bullets.Length + " bullets");
        
        foreach (BeamShot bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
    }

    public void RestartGame()
    {
        DestroyAllDrones();
        currentRound = 1;
        StartGame();
    }

    private void SpawnDrones(float waitTime)
    {
        gameState = GameState.Spawning;

        currentOneHitKilled = 0;
        currentTwoHitsKilled = 0;
        currentExplosiveKilled = 0;

        StartCoroutine(ActivateDroneFromList(waitTime));
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

    private void SpawnDrone(GameObject dronePrefab, int i)
    {
        Vector3 randomPosition = GetRandomPosition(i);
        Quaternion randomRotation = GetRandomDirection();
        GameObject newDrone = Instantiate(dronePrefab, randomPosition, randomRotation);
        newDrone.GetComponent<BaseDroneController>().SetSpawner(this);
        droneList.Add(newDrone);
        newDrone.SetActive(false);
    }

    private float CalculateRandomInterval(int totalDrones)
    {
        float spawnInterval = spawnTime / totalDrones;
        float randomTime = Random.Range(0f, spawnInterval * 2f);
        spawnTime -= spawnInterval;
        return randomTime;
    }

    private IEnumerator ActivateDroneFromList(float waitTime)
    {
        CalculateDroneAmount();

        yield return new WaitForSeconds(waitTime);

        Debug.Log("Activate drones called");

        // Add OneHit drones
        for (int i = 0; i < currentOneHitToSpawn; i++)
        {
            SpawnDrone(oneHitDrone, i);
        }

        // Add TwoHits drones
        for (int i = 0; i < currentTwoHitsToSpawn; i++)
        {
            SpawnDrone(twoHitsDrone, i);
        }

        // Add Explosive drones
        for (int i = 0; i < currentExplosiveToSpawn; i++)
        {
            SpawnDrone(explosiveDrone, i);
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
        if(isTutorial)
        {
            //Tutorial Test Wave Shuffle
            for (int i = 0; i < droneList.Count - 1; i++)
            {
                int randomIndex = Random.Range(0, droneList.Count - 1);
                GameObject temp = droneList[i];
                droneList[i] = droneList[randomIndex];
                droneList[randomIndex] = temp;
            }
        }

        else
        {
            // Normal Shuffle
            for (int i = 0; i < droneList.Count; i++)
            {
                int randomIndex = Random.Range(0, droneList.Count);
                GameObject temp = droneList[i];
                droneList[i] = droneList[randomIndex];
                droneList[randomIndex] = temp;
            }
        }
    }

    private Vector3 GetRandomPosition(int i)
    {
        bool even = i % 2 == 0;

        float randomAngle;

        if (even)
        {
            randomAngle = Random.Range(minAngle, -gapAngle / 2f) * Mathf.Deg2Rad;
        }

        else
        {
            randomAngle = Random.Range(gapAngle / 2f, maxAngle) * Mathf.Deg2Rad;
        }
       
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
        droneList.Remove(drone);

        if (gameState == GameState.NoGame) return;

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

        if(!AreAllDronesKilled() && droneList.Count > 0)
        {
            SpawnNext();
        }

        if (AreAllDronesKilled())
        {
            gameState = GameState.NextRound;
        }
    }

    private void SpawnNext()
    {
        for(int i = 0; i < droneList.Count; i++)
        {
            if (!droneList[i].activeSelf)
            {
                droneList[i].SetActive(true);
                return;
            }
        }
        Debug.Log("Current list count of drones: " + droneList.Count);
    }

    private bool AreAllDronesKilled()
    {
        return currentOneHitToSpawn - currentOneHitKilled <= 0 && 
            currentTwoHitsToSpawn - currentTwoHitsKilled <= 0 && 
            currentExplosiveToSpawn - currentExplosiveKilled <= 0;
    }

    private void SetDroneActive(int index)
    {
        if(index < droneList.Count && index >= 0)
        {
            droneList[index].SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {

        if (playerTransform != null && debug)
        {
            // Draw the spheres
            Gizmos.color = sphere1Color;
            Gizmos.DrawWireSphere(playerTransform.position, sphere1Radius);

            Gizmos.color = sphere2Color;
            Gizmos.DrawWireSphere(playerTransform.position, sphere2Radius);

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

    private List<GameObject> GetSortedActiveDroneList()
    {
        List<GameObject> activeDrones = new();

        foreach (GameObject drone in droneList)
        {
            if (drone.activeInHierarchy)
            {
                activeDrones.Add(drone);             
            }
        }

        // Sort the active drones based on distanceToPlayer
        activeDrones.Sort(new DistanceToPlayerComparer());

        return activeDrones;
    }

    public BaseDroneController GetClosestActiveDrone()
    {
        List<GameObject> activeDrones = new();

        foreach (GameObject drone in droneList)
        {
            if (drone.activeInHierarchy)
            {
                activeDrones.Add(drone);
            }
        }

        // Sort the active drones based on distanceToPlayer
        activeDrones.Sort(new DistanceToPlayerComparer());

        return activeDrones.Count > 0 ? activeDrones[0].GetComponent<BaseDroneController>() : null;
    }

    private IEnumerator SetPriority()
    {
        List<GameObject> list = GetSortedActiveDroneList();

        if(list.Count > 0)
        {
            int highPriority = Mathf.Min(maxSlotsHighPriority, list.Count);
            int mediumPriority = Mathf.Min(maxSlotsMediumPriority, list.Count - highPriority);

            for (int i = 0; i < highPriority; i++)
            {
                GameObject drone = list[i];
                drone.GetComponent<BaseDroneController>().SwitchLevel(PriorityLevel.high);
                Debug.Log("high priority set");
            }

            if(highPriority < list.Count)
            {
                for (int i = highPriority; i < highPriority + mediumPriority; i++)
                {
                    GameObject drone = list[i];
                    drone.GetComponent<BaseDroneController>().SwitchLevel(PriorityLevel.medium);
                    Debug.Log("medium priority set");
                }
            }

            if(highPriority + mediumPriority < list.Count)
            {
                for (int i = highPriority + mediumPriority; i < list.Count; i++)
                {
                    GameObject drone = list[i];
                    drone.GetComponent<BaseDroneController>().SwitchLevel(PriorityLevel.low);
                    Debug.Log("low priority set");
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(SetPriority());
    }

    private void ActivateTowers()
    {
        TowerManager[] towers = FindObjectsOfType<TowerManager>();

        if(towers.Length > 0)
        {
            foreach (TowerManager tower in towers)
            {
                tower.StartTowerVFX();
            }
        }
    }

    private void DeactivateTowers()
    {
        TowerManager[] towers = FindObjectsOfType<TowerManager>();

        if (towers.Length > 0)
        {
            foreach (TowerManager tower in towers)
            {
                tower.StopTowerVFX();
            }
        }
    }

    public void SetTutorialManager(TutorialManager value)
    {
        tutorialManager = value;
        if(value != null)
        {
            isTutorial = true;
        }
    }

    public void AddDrone(GameObject drone)
    {
        droneList.Add(drone);
    }
}
