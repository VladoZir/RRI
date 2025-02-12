using UnityEngine;

public class FlyingController : MonoBehaviour
{
    public float liftForce = 5f; // Adjust lift force
    private Rigidbody2D rb;

    public Sprite normalSprite; // Spaceship without fire
    public Sprite fireSprite;   // Spaceship with fire

    private SpriteRenderer spriteRenderer;

    public float tiltSpeed = 4f; // Controls how quickly the ship rotates
    public float maxTiltAngle = 30f; // Maximum tilt in degrees

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // When the left mouse button is pressed
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, liftForce);
            spriteRenderer.sprite = fireSprite; // Show fire sprite
        }
        else
        {
            spriteRenderer.sprite = normalSprite; // Show normal sprite
        }

        RotateShip();
    }

    void RotateShip()
    {
        float tiltAngle = Mathf.Clamp(rb.linearVelocity.y * maxTiltAngle / liftForce, -maxTiltAngle, maxTiltAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, tiltAngle), Time.deltaTime * tiltSpeed);
    }
}
