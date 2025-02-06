using UnityEngine;

public class ParallaxElement : MonoBehaviour
{
    public float parallaxSpeed = 1.0f; // Speed of the parallax effect
    private Vector3 previousCameraPosition;

    void Start()
    {
        // Store the initial camera position
        previousCameraPosition = Camera.main.transform.position;
    }

    void Update()
    {
        // Get the difference in position between the previous and current camera positions
        Vector3 cameraMovement = Camera.main.transform.position - previousCameraPosition;

        // Move the element based on the camera's movement
        transform.position += new Vector3(cameraMovement.x * parallaxSpeed, 0, 0);

        // Update the previous camera position for the next frame
        previousCameraPosition = Camera.main.transform.position;
    }
}
