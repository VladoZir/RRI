using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the player prefab
    public Vector3 spawnPosition = new Vector3(0f, 1f, 0f); // Define spawn position

    void Start()
    {
        // Instantiate the player at the spawn position
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }
}
