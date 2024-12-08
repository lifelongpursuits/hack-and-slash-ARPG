using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float respawnDelay = 3f;

    private int currentEnemies;

    private void Start()
    {
        // Spawn initial enemies
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (currentEnemies >= maxEnemies) return;

        // Get random position within spawn radius
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Create enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemies++;

        // Subscribe to enemy death
        if (enemy.TryGetComponent<EnemyHealth>(out var health))
        {
            health.OnEnemyDeath += HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath()
    {
        currentEnemies--;
        Invoke(nameof(SpawnEnemy), respawnDelay);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize spawn area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
} 