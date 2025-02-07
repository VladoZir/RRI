using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float speed = 2f; // Movement speed
    public float detectionRange = 5f; // Range at which the spider detects and follows the player
    public float attackRange = 1f; // Distance at which the spider attack
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
            {
                // Move towards the player's X position, no vertical movement
                Vector2 direction = (player.position.x - transform.position.x > 0) ? Vector2.right : Vector2.left;
                rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y); // Keep the same vertical velocity
            }
            else
            {
                // Stop movement if out of range or within attack range
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                Debug.Log("Spider attacked the player!");
            }
        }
    }

    // Draw the detection and attack range in the scene view for debugging
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Attack range
    }
}
