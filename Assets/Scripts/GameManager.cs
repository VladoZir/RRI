using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab; 
    public Vector3 spawnPosition = new Vector3(5f, 0f, 0f); 

    public Camera mainCamera;

    public GameObject enemy;

    private GameObject currentPlayer;

    public AudioSource collectAudio;

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
    }

    public void UpgradePlayer(GameObject newPrefab, GameObject oldPlayer)
    {
        Vector3 position = oldPlayer.transform.position; // Store player position
        Destroy(oldPlayer); // Remove the old player
        currentPlayer = Instantiate(newPrefab, position, Quaternion.identity); // Spawn new player at the same position
        collectAudio.Play();

        // Assign the new player to the camera
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.player = currentPlayer;
            }
        }

        // Link the bow to the PlayerController (if the new player prefab has the bow)
        PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Transform bowTransform = currentPlayer.transform.Find("Bow"); // Assuming Bow is a child of the player prefab
            if (bowTransform != null)
            {
                Debug.Log("bow found");
                playerController.bow = bowTransform; // Link the bow to the PlayerController
            }
        }
    }


}
