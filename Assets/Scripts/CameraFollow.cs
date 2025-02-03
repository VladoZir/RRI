using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public Vector3 offset;    // Offset from the player to position the camera

    void Start()
    {
        // If the player is not set in the inspector, try to find it in the scene
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); // Make sure your player has the "Player" tag
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Update the camera position to follow the player with the offset
            transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, transform.position.z);
        }
    }
}
