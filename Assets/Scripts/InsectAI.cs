using UnityEngine;

public class InsectAI : MonoBehaviour
{
    private Transform player;
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public int damageAmount = 10;
    private Rigidbody2D rb;
    private bool isFacingRight = true; // Tracks current facing direction

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                Debug.Log("Insect found the player!");
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange && distanceToPlayer > attackRange)
        {
            Vector2 direction = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
            rb.linearVelocity = new Vector2(direction.x * speed, 0f);

            // Flip the sprite if moving in the opposite direction
            if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip by inverting the X scale
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Insect attacked the player!");
            }
        }
    }
}
