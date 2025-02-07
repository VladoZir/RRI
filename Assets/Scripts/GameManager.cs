using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab; 
    public Vector3 spawnPosition = new Vector3(5f, 0f, 0f); 

    public Camera mainCamera;

    public GameObject enemy;

    private GameObject currentPlayer; // Track the spawned player

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        SpawnPlayer(playerPrefab);

    }

    public void SpawnPlayer(GameObject prefab)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer); // Destroy previous player instance
        }

        currentPlayer = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Assign the new player to the camera
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.player = currentPlayer;
            }
        }

        // Assign player to enemy AI if needed
        if (enemy != null)
        {
            DinoBossAI enemyAI = enemy.GetComponent<DinoBossAI>();
            if (enemyAI != null)
            {
                enemyAI.player = currentPlayer.transform;
            }
        }
    }

    public void UpgradePlayer(GameObject newPrefab, GameObject oldPlayer)
    {
        Vector3 position = oldPlayer.transform.position; // Store player position
        Destroy(oldPlayer); // Remove the old player
        SpawnPlayer(newPrefab); // Spawn new player at the same position
        currentPlayer.transform.position = position; // Restore position
    }

}
