using UnityEngine;

public class SeamlessParallax : MonoBehaviour
{
    public Transform cam; 
    public float parallaxEffectMultiplier = 0.5f; 
    public float teleportThreshold = 20f; 

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

        float deltaX = cam.position.x - lastCameraPosition.x;

        Vector3 parallaxOffset = new Vector3(-deltaX * parallaxEffectMultiplier, 0, 0);
        transform.position += parallaxOffset;

        //Debug.Log($"Camera Position: {cam.position.x}, Texture Position: {transform.position.x}");

        if (Mathf.Abs(transform.position.x - cam.position.x) >= teleportThreshold)
        {
            if (transform.position.x > cam.position.x)
            {
                transform.position = new Vector3(transform.position.x - teleportThreshold, transform.position.y, transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x + teleportThreshold, transform.position.y, transform.position.z);
            }
        }

        lastCameraPosition = cam.position;
    }
}
