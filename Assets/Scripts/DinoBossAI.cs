using UnityEngine;
using System.Collections;

public class DinoBossAI : MonoBehaviour
{
    public float speed = 3f;            // Normal movement speed
    public float detectionRange = 25f;   // Range to detect the player
    public float dashSpeed = 8f;        // Speed during dash
    public float dashCooldown = 3f;     // Time before the dino can dash again
    public float dashDuration = 0.5f;   // Duration of the dash
    private bool isDashing = false;     // Flag to check if dashing
    private bool isCoolingDown = false; // Flag for cooldown
    private float cooldownTimer = 0f;   // Timer for cooldown

    public Transform player;            // Player reference
    private Animator animator;          // Animator reference
    private Rigidbody2D rb;             // Rigidbody for physics-based movement

   

    void Start()
    {
        animator = GetComponent<Animator>();  // Get animator component
        rb = GetComponent<Rigidbody2D>();    // Get rigidbody component
    }

    void Update()
    {
        if (player == null) return;  // Ensure the player is set

        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;  // Reset cooldown
            }
            return; // Exit the update loop if the enemy is in cooldown
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If player is within range and not dashing, start dash
        if (distanceToPlayer <= detectionRange && !isDashing)
        {
            StartCoroutine(DashTowardsPlayer());
        }
    }

    private IEnumerator DashTowardsPlayer()
    {
        isDashing = true;
        isCoolingDown = true; // Start cooldown

        animator.SetTrigger("Dash"); // Play dash animation

        Vector2 dashDirection = (player.position - transform.position).normalized;
        float dashEndTime = Time.time + dashDuration;

        // Move the dino quickly towards the player
        while (Time.time < dashEndTime)
        {
            rb.linearVelocity = dashDirection * dashSpeed; // Move the dino during the dash
            yield return null;
        }

        rb.linearVelocity = Vector2.zero; // Stop movement after dash

        

        isDashing = false;
        cooldownTimer = dashCooldown; // Set cooldown timer
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(20); // Dino deals 20 damage on hit
            }

            isCoolingDown = true;
            cooldownTimer = dashCooldown;
        }
    }
}
