using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    
    public float slideSpeed = 2f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isOnWall;
    private Animator anim;
    public Transform bow;
    public AudioSource walkGrassAudio;
    public AudioSource jumpAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component missing from player!");
        }
    }

    void Update()
    {
        Move();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Only apply wall slide if we're not on the ground
        if (isOnWall && !isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -slideSpeed);
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("IsJumping", !isGrounded);
        }
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

        // Only apply horizontal movement if not on a wall or if on ground
        if (!isOnWall || isGrounded)
        {
            rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
        }

        if (moveInput != 0 && isGrounded && walkGrassAudio != null && !walkGrassAudio.isPlaying)
        {
            walkGrassAudio.Play();
        }
        else if ((moveInput == 0 || !isGrounded) && walkGrassAudio != null && walkGrassAudio.isPlaying)
        {
            walkGrassAudio.Stop();
        }

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            FlipBow(false);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            FlipBow(true);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        if (jumpAudio != null && !jumpAudio.isPlaying)
        {
            jumpAudio.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            bool foundGround = false;

            // First check for ground collision
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);

                // Check for ground collision (angle less than 45 degrees from up)
                if (angle < 45f)
                {
                    isGrounded = true;
                    isOnWall = false;  // Clear wall state when grounded
                    if (anim != null)
                    {
                        anim.SetBool("IsJumping", false);
                    }
                    foundGround = true;
                    break;  // Exit the loop if we found ground
                }
            }

            // Only check for wall if we didn't find ground
            if (!foundGround)
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    float angle = Vector2.Angle(contact.normal, Vector2.up);
                    // Check for wall collision (angle close to 90 degrees)
                    if (angle > 85f)
                    {
                        isOnWall = true;
                        break;
                    }
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Recheck ground collision during continuous contact
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);
                if (angle < 45f)
                {
                    isGrounded = true;
                    isOnWall = false;  // Clear wall state when grounded
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isOnWall = false;
            if (anim != null)
            {
                anim.SetBool("IsJumping", true);
            }
        }
    }
}