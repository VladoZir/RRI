using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public GameObject playerSpawn; // Reference to the player spawn object
    public Vector3 offset;    // Offset from the player to position the camera

    void Start()
    {
        // If the playerSpawn is set in the inspector, use that as the initial target
        if (playerSpawn != null)
        {
            player = playerSpawn; // Set camera to follow playerSpawn object initially
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Update the camera position to follow the player (or playerSpawn) with the offset
            transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, transform.position.z);
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        player = newTarget; // Set the target the camera should follow
    }
}
