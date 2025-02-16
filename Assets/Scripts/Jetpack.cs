using UnityEngine;

public class JetpackController : MonoBehaviour
{
    public float jetpackForce = 10f;          // The vertical force applied by the jetpack
    public float initialBoostForce = 15f;     // The extra force for the initial boost
    public float horizontalBoostFactor = 1.5f; // Factor to speed up horizontal movement
    public Sprite idleSprite;                 // Sprite when jetpack is idle
    public Sprite activeSprite;               // Sprite when jetpack is in use

    private Rigidbody2D playerRigidbody;       // Player's Rigidbody2D
    private SpriteRenderer jetpackSpriteRenderer; // Jetpack's SpriteRenderer
    private bool isBoosting = false;          // Track if the boost is applied

    public AudioSource jetpackAudio; // Reference to AudioSource

    void Start()
    {
        // Get the Rigidbody2D component from the Player object (parent of the jetpack)
        playerRigidbody = GetComponentInParent<Rigidbody2D>();
        if (playerRigidbody == null)
        {
            Debug.LogError("No Rigidbody2D found on the parent Player object!");
        }

        // Get the SpriteRenderer of the jetpack object (where you want the sprite to change)
        jetpackSpriteRenderer = GetComponent<SpriteRenderer>();
        if (jetpackSpriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the Jetpack object!");
        }

        if (jetpackAudio == null)
        {
            jetpackAudio = GetComponent<AudioSource>(); // Get the AudioSource
        }
    }

    void Update()
    {
        // Check if the spacebar is being held down
        if (Input.GetKey(KeyCode.Space)) // Play only once when Space is first pressed
        {
            if (!jetpackAudio.isPlaying)
            {
                jetpackAudio.Play();
            }
            ActivateJetpack();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) // Stop when Space is released
        {
            jetpackAudio.Stop();
            DeactivateJetpack();
        }
    }

    void ActivateJetpack()
    {
        // Apply initial boost only once
        if (!isBoosting)
        {
            playerRigidbody.AddForce(Vector2.up * initialBoostForce, ForceMode2D.Force);
            isBoosting = true; // Prevent applying boost again
        }

        // Apply the regular jetpack force vertically
        playerRigidbody.AddForce(Vector2.up * jetpackForce, ForceMode2D.Force);

        // Speed up the player horizontally if jetpack is active
        Vector2 currentVelocity = playerRigidbody.linearVelocity;
        currentVelocity.x *= horizontalBoostFactor;  // Increase horizontal speed
        playerRigidbody.linearVelocity = currentVelocity;

        // Change sprite to the active one when the jetpack is being used
        if (jetpackSpriteRenderer != null)
        {
            jetpackSpriteRenderer.sprite = activeSprite;
        }
    }

    void DeactivateJetpack()
    {
        // Reset the sprite to idle when the spacebar is not held
        if (jetpackSpriteRenderer != null)
        {
            jetpackSpriteRenderer.sprite = idleSprite;
        }

        // Reset the boost when the spacebar is released
        if (isBoosting)
        {
            isBoosting = false;
        }
    }
}
