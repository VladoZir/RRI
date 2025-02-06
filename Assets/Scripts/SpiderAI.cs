using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float speed = 2f; // Movement speed
    public float attackRange = 1f; // Distance at which the spider attacks
    public int damageAmount = 10; // Damage dealt to the player
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the player is assigned if not manually set
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
            }
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Get horizontal distance to the player (ignore Y)
            float horizontalDistance = Mathf.Abs(transform.position.x - player.position.x);

            if (horizontalDistance > attackRange)
            {
                // Move towards the player's X position, no vertical movement (Y-axis stays the same)
                Vector2 direction = (player.position.x - transform.position.x > 0) ? Vector2.right : Vector2.left;
                rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y); // Keep the same vertical velocity
            }
            else
            {
                // Stop movement when within attack range
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the spider collided with the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Spider attacked the player!");
            }
        }
    }
}
