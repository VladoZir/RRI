using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;

    [SerializeField] Transform bow;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        bow = new GameObject("Bow").transform;
        bow.SetParent(transform); // Set the bow as a child of the player
        bow.localPosition = Vector3.zero;
    }

    void Update()
    {
        Move();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        // Update Animator parameters
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x)); // Use absolute value for movement speed
        anim.SetBool("IsJumping", !isGrounded); // IsJumping is true while in the air

    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // Use velocity instead of linearVelocity

        if (moveInput > 0)
        {
            // Flip sprite to the right (no need to modify local scale)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        }
        else if (moveInput < 0)
        {
            // Flip sprite to the left (no need to modify local scale)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        bow.localScale = new Vector3(1f, 1f, 1f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("IsJumping", false);
        }
    }
}