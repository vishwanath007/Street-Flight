using UnityEngine;
using System.Collections.Generic; 

public class ObstacleGenerator : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnXRange = 5f; 
    public float spawnZRange = 5f; 
    public int initialObstacleCount = 10;
    public float forwardSpawnDistance = 50f; 

    private Transform playerTransform;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float lastSpawnZ = 0f;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        InitialObstacleBurst();
    }

    void Update()
    {
        CheckForObstacleRemoval();
        GenerateForwardObstacles();
    }

    void InitialObstacleBurst()
    {
        for (int i = 0; i < initialObstacleCount; i++)
        {
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        float randomX = Random.Range(-spawnXRange, spawnXRange);
        float randomZ = Random.Range(-spawnZRange, spawnZRange);
        Vector3 spawnPosition = new Vector3(
                playerTransform.position.x + randomX,   // Offset from the player's X
                0f,                                     // Fixed Y at 0
                playerTransform.position.z + randomZ    // Offset from the player's Z
            );

        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }

    void CheckForObstacleRemoval()
    {
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i].transform.position.z < playerTransform.position.z - 5f) // Adjust offset if needed
            {
                Destroy(activeObstacles[i]); 
                activeObstacles.RemoveAt(i);
            }
        }
    }

    void GenerateForwardObstacles()
    {
        if (playerTransform.position.z - lastSpawnZ >= forwardSpawnDistance / 2f) // Spawn when nearing halfway
        {
            lastSpawnZ = playerTransform.position.z; 
            SpawnObstacle(); 
        }
    }
}
