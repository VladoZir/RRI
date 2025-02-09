using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator anim;

    public Transform bow;

    public AudioSource walkGrassAudio;
    public AudioSource jumpAudio;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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

    void FlipBow(bool flipLeft)
    {
        if (bow != null) 
        {
            if (flipLeft)
            {
                bow.localScale = new Vector3(-1f, 1f, 1f);  
            }
            else
            {
                bow.localScale = new Vector3(1f, 1f, 1f);  
            }
        }
    }


    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        if (moveInput != 0 && isGrounded && !walkGrassAudio.isPlaying)
        {
            walkGrassAudio.Play();
        }
        // Stop the walking sound if player stops moving or is in the air
        else if ((moveInput == 0 || !isGrounded) && walkGrassAudio.isPlaying)
        {
            walkGrassAudio.Stop();
        }

        if (moveInput > 0)
        {
            // Flip sprite to the right (no need to modify local scale)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            FlipBow(false);
        }
        else if (moveInput < 0)
        {
            // Flip sprite to the left (no need to modify local scale)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            FlipBow(true);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (!jumpAudio.isPlaying) 
        {
            jumpAudio.Play();
        }
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