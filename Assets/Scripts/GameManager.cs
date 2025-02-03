using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the player prefab
    public Vector3 spawnPosition = new Vector3(0f, 1f, 0f); // Define spawn position

    public Camera mainCamera;

    void Start()
    {
        // Instantiate the player at the spawn position
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        // Set the player's tag to "Player" for the CameraFollow script to work
        player.tag = "Player";

        // If you have the CameraFollow script, set the player reference in the camera follow script
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.player = player;  // Assign the player to the camera follow script
            }
        }
    }
}
