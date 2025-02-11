using UnityEngine;

public class FlyingController : MonoBehaviour
{
    public float liftForce = 5f; // Adjust this to control how strong the upward force is
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left mouse button is pressed
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, liftForce);
        }
    }

   
}
