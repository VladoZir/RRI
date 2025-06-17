using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public float wallBounceForce = 3f;  

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isOnWall;
    private Animator anim;
    public Transform bow;
    public SpaceGun spaceGun;
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

        UpdateBowRotation();
        Move();
        // Only allow jumping if grounded AND not touching a wall
        if (Input.GetButtonDown("Jump") && isGrounded && !isOnWall)
        {
            Jump();
        }

        if (spaceGun != null)
        {
            spaceGun.AimAtMouse();  // Use the SpaceGun's aiming logic
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            anim.SetBool("IsJumping", !isGrounded);
        }
    }



    void UpdateBowRotation()
    {
        if (bow == null) return;

        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Get direction from bow to mouse
        Vector2 direction = (mousePos - bow.position).normalized;

        // Calculate angle to rotate towards mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply rotation (bow should always face the mouse)
        bow.rotation = Quaternion.Euler(0, 0, angle);

        // Flip the bow's scale on X when the player turns
        bool facingLeft = transform.localScale.x < 0;
        bow.localScale = facingLeft ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
    }

    void Move()
    {
        float moveInput = Input.GetAxis("Horizontal");

        // Apply horizontal movement if not on a wall or if on ground
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
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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

            // Check for wall collision and apply bounce
            if (!foundGround)
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    float angle = Vector2.Angle(contact.normal, Vector2.up);
                    // Check for wall collision (angle close to 90 degrees)
                    if (angle > 85f)
                    {
                        isOnWall = true;
                        // Apply bounce force in the direction of the wall normal
                        Vector2 bounceDirection = contact.normal;
                        rb.linearVelocity = new Vector2(bounceDirection.x * wallBounceForce, rb.linearVelocity.y);
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
            bool foundGround = false;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                float angle = Vector2.Angle(contact.normal, Vector2.up);
                if (angle < 45f)
                {
                    isGrounded = true;
                    isOnWall = false;  // Clear wall state when grounded
                    foundGround = true;
                    break;
                }
            }

            // Only check for wall state if not on ground
            if (!foundGround)
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    float angle = Vector2.Angle(contact.normal, Vector2.up);
                    if (angle > 85f)
                    {
                        isOnWall = true;
                        break;
                    }
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