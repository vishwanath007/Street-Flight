using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab; // Assign the asteroid prefab
    public Transform playerTransform; // Assign your player's transform
    public float spawnRate = 2f; // How often asteroids spawn
    public float spawnDistance = 10f; // Distance from the player to spawn asteroids
    public float asteroidSpeed = 50f; // Speed of the asteroids
    public Vector2 spawnSizeRange = new Vector2(0.5f, 2f); // Size range of the asteroids

    private float nextSpawnTime;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }
    void Update()
    {
        if (Time.time >= nextSpawnTime && playerTransform != null)
        {
            SpawnAsteroid();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnAsteroid()
    {
        Vector3 spawnDirection = Random.onUnitSphere; // For 3D
        // For 2D use something like: Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPoint = playerTransform.position + spawnDirection * spawnDistance;

        // Correct Y for 2D games or if you want to limit the spawn height
        // spawnPoint.y = playerTransform.position.y; // Uncomment and adjust for 2D or specific 3D needs

        Quaternion rotation = Quaternion.LookRotation(playerTransform.position - spawnPoint);

        GameObject asteroid = Instantiate(asteroidPrefab, spawnPoint, rotation);
        Rigidbody rb = asteroid.GetComponent<Rigidbody>();
        // For 2D, use Rigidbody2D rb = asteroid.GetComponent<Rigidbody2D>();

        // Adjust size randomly
        float size = Random.Range(spawnSizeRange.x, spawnSizeRange.y);
        asteroid.transform.localScale = new Vector3(size, size, size);

        // Move asteroid towards the player
        Vector3 moveDirection = (playerTransform.position - asteroid.transform.position).normalized;
        rb.velocity = moveDirection * asteroidSpeed;
        // For 2D, use rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * asteroidSpeed;
    }
}
