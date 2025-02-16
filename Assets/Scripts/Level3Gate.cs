using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class RoomTileActivator : MonoBehaviour
{
    public Tilemap tilemap; // Assign in the inspector
    private bool activated = false; // Prevents re-triggering

    private List<GameObject> enemiesInRoom = new List<GameObject>();
    public GameObject enemyPrefab; // Assign your enemy prefab in the inspector
    public Transform[] spawnPoints; // Assign multiple spawn points inside the room

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            tilemap.gameObject.SetActive(true); // Close the gate
            activated = true; // Prevents further changes

            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        enemiesInRoom.Clear();

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemiesInRoom.Add(enemy);

            // Listen for enemy death if they have an EnemyAI script
            if (enemy.TryGetComponent(out EnemyAI enemyAI))
            {
                enemyAI.OnDeath += EnemyDefeated;
            }
        }
    }

    private void EnemyDefeated(GameObject enemy)
    {
        enemiesInRoom.Remove(enemy);

        if (enemiesInRoom.Count == 0)
        {
            OpenGates(); // Open the gate when all enemies are defeated
        }
    }

    private void OpenGates()
    {
        tilemap.gameObject.SetActive(false); // Open the gate
    }
}
