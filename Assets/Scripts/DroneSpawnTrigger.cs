using Pathfinding;
using UnityEngine;

public class DroneSpawnTrigger : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    private Collider2D triggerCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpawnEnemies();

            triggerCollider.enabled = false;
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) return;

        Transform playerTransform = GameManager.Instance?.currentPlayer?.transform;

        int spawnCount = Mathf.Min(10, spawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnLocation = spawnPoints[i];

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);

            // Immediately set the AI target if the player is already detected
            AIDestinationSetter aiDestination = spawnedEnemy.GetComponent<AIDestinationSetter>();
            if (aiDestination != null && playerTransform != null)
            {
                aiDestination.target = playerTransform;
            }
        }
    }

}
