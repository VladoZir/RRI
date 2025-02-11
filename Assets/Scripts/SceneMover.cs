using UnityEngine;

public class SceneMover : MonoBehaviour
{
    public float scrollSpeed = 2f; // Speed at which the scene moves toward the player
    public GameObject player; // Reference to the player GameObject
    private Vector3 startPosition; // Starting position of the scene
    public bool isPaused = false; // Flag to pause the scene movement

    void Start()
    {
        // Store the initial position of the scene
        startPosition = transform.position;
    }

    void Update()
    {
        if (!isPaused) // Only move the scene if it's not paused
        {
            // Move the scene toward the player by modifying the scene's position
            Vector3 scenePosition = transform.position;

            // Move the scene left (towards the player)
            scenePosition.x -= scrollSpeed * Time.deltaTime;

            // Apply the new position to the scene
            transform.position = scenePosition;
        }
    }

    public void PauseSceneMovement(bool pause)
    {
        isPaused = pause;
    }
}
