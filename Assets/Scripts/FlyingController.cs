using UnityEngine;

public class FlyingController : MonoBehaviour
{
    public float liftForce = 5f; 
    private Rigidbody2D rb;

    public Sprite normalSprite; 
    public Sprite fireSprite;  

    private SpriteRenderer spriteRenderer;

    public float tiltSpeed = 4f; 
    public float maxTiltAngle = 30f;

    public float pushForce = 3f;

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
        if (Input.GetMouseButton(0)) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, liftForce);
            spriteRenderer.sprite = fireSprite; 
        }
        else
        {
            spriteRenderer.sprite = normalSprite; 
        }

        RotateShip();
    }

    void RotateShip()
    {
        float tiltAngle = Mathf.Clamp(rb.linearVelocity.y * maxTiltAngle / liftForce, -maxTiltAngle, maxTiltAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, tiltAngle), Time.deltaTime * tiltSpeed);
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("TopBarrier"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
            rb.AddForce(Vector2.down * pushForce, ForceMode2D.Impulse); 
        }
        else if (collision.gameObject.CompareTag("BottomBarrier"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
            rb.AddForce(Vector2.up * pushForce, ForceMode2D.Impulse); 
        }
    }
    */
}
