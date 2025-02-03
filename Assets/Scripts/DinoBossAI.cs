using UnityEngine;

public class DinoBossAI : MonoBehaviour
{
    public float speed = 3f;         // Movement speed of the enemy
    public float detectionRange = 5f; // Distance at which the enemy starts to follow the player
    public Transform player;         // Reference to the player's transform

    private bool isCoolingDown = false; // Flag to check if the enemy is in cooldown
    private float cooldownTimer = 0f;   // Timer to track the cooldown duration
    public float cooldownDuration = 2f; // Duration for which the enemy stops chasing the player after a collision

    private void Update()
    {
        if (player == null) return;  // Ensure the player is set

        // If the enemy is cooling down, skip the movement logic
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isCoolingDown = false;  // Reset cooldown and allow following again
            }
            return; // Exit the update loop if the enemy is in cooldown
        }

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If the player is within the detection range, move towards the player
        if (distanceToPlayer <= detectionRange)
        {
            // Move towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the boss collides with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(10);  // You can adjust the damage value
            }

            // Start the cooldown timer and stop chasing the player
            isCoolingDown = true;
            cooldownTimer = cooldownDuration;
        }
    }
}
