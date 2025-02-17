using UnityEngine;
public class JetpackController : MonoBehaviour
{
    public float jetpackForce = 12.5f;  // Increased force
    public float maxVerticalSpeed = 15f;
    public float horizontalBoostFactor = 1.2f;
    public Sprite idleSprite;
    public Sprite activeSprite;
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer jetpackSpriteRenderer;
    private bool jetpackActive = false;
    public AudioSource jetpackAudio;

    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        if (playerRigidbody == null)
        {
            Debug.LogError("No Rigidbody2D found on the parent Player object!");
        }
        jetpackSpriteRenderer = GetComponent<SpriteRenderer>();
        if (jetpackSpriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the Jetpack object!");
        }
        if (jetpackAudio == null)
        {
            jetpackAudio = GetComponent<AudioSource>();
        }
    }

    void FixedUpdate()  // Changed to FixedUpdate for physics
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!jetpackActive)
            {
                jetpackActive = true;
                if (!jetpackAudio.isPlaying)
                {
                    jetpackAudio.Play();
                }
            }
            ActivateJetpack();
        }
        else
        {
            if (jetpackActive)
            {
                jetpackActive = false;
                jetpackAudio.Stop();
                DeactivateJetpack();
            }
        }
    }

    void ActivateJetpack()
    {
        // Calculate the force needed to overcome gravity and provide lift
        float gravityCompensation = -Physics2D.gravity.y * playerRigidbody.mass;
        float upwardForce = gravityCompensation + jetpackForce;

        // Apply force to counteract gravity and then add the jetpack force
        playerRigidbody.AddForce(Vector2.up * upwardForce, ForceMode2D.Force);

        // Clamp vertical velocity to prevent excessive speeds
        if (playerRigidbody.linearVelocity.y > maxVerticalSpeed)
        {
            Vector2 clampedVelocity = playerRigidbody.linearVelocity;
            clampedVelocity.y = maxVerticalSpeed;
            playerRigidbody.linearVelocity = clampedVelocity;
        }

        // Apply horizontal boost if the player is moving horizontally
        if (Mathf.Abs(playerRigidbody.linearVelocity.x) > 0.1f)
        {
            Vector2 horizontalBoost = playerRigidbody.linearVelocity;
            horizontalBoost.x *= horizontalBoostFactor;
            playerRigidbody.linearVelocity = horizontalBoost;
        }

        // Update sprite
        if (jetpackSpriteRenderer != null)
        {
            jetpackSpriteRenderer.sprite = activeSprite;
        }
    }

    void DeactivateJetpack()
    {
        // Reset sprite
        if (jetpackSpriteRenderer != null)
        {
            jetpackSpriteRenderer.sprite = idleSprite;
        }
    }
}