using UnityEngine;

public class SeamlessParallax : MonoBehaviour
{
    public Transform cam; // Camera's transform
    public float parallaxEffectMultiplier = 0.5f; // Parallax effect speed
    public float teleportThreshold = 20f; // Threshold in units (1 unit = 16px)

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Camera not assigned!");
            return;
        }

        lastCameraPosition = cam.position;
    }

    void Update()
    {
        if (cam == null) return;

        // Calculate camera movement delta
        float deltaX = cam.position.x - lastCameraPosition.x;

        // Apply parallax effect to the texture
        Vector3 parallaxOffset = new Vector3(deltaX * parallaxEffectMultiplier, 0, 0);
        transform.position += parallaxOffset;

        // Debug the positions
        //Debug.Log($"Camera Position: {cam.position.x}, Texture Position: {transform.position.x}");

        // Check if the texture has moved beyond the threshold
        if (Mathf.Abs(transform.position.x - cam.position.x) >= teleportThreshold)
        {
            // Teleport the texture 1 unit to the left or right depending on which direction we moved
            if (transform.position.x > cam.position.x)
            {
                // Teleport to the left
                transform.position = new Vector3(transform.position.x - teleportThreshold, transform.position.y, transform.position.z);
            }
            else
            {
                // Teleport to the right
                transform.position = new Vector3(transform.position.x + teleportThreshold, transform.position.y, transform.position.z);
            }
        }

        // Update the last camera position
        lastCameraPosition = cam.position;
    }
}
