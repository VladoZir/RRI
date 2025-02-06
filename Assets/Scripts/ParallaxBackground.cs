using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cam; // Assign the Camera Transform
    public float parallaxEffectMultiplier = 0.5f; // Adjust per layer for different speeds
    public bool isForeground = false; // Enable this for the front 3 layers

    private float textureUnitSizeX;
    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cam.position;

        // Get the width of the sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        textureUnitSizeX = spriteRenderer.bounds.size.x;
    }

    void Update()
    {
        // Calculate parallax movement
        Vector3 deltaMovement = cam.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, 0, 0);

        // If this is a foreground element, apply slight vertical movement
        if (isForeground)
        {
            transform.position += new Vector3(0, -deltaMovement.y * 0.2f, 0); // Adjust vertical effect
        }

        lastCameraPosition = cam.position;

        // Wrap background when it goes out of bounds
        if (Mathf.Abs(cam.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offset = (cam.position.x - transform.position.x) > 0 ? textureUnitSizeX * 2 : -textureUnitSizeX * 2;
            transform.position += new Vector3(offset, 0, 0);
        }
    }
}
