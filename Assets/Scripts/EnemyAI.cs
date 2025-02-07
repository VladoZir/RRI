using UnityEngine;

public class EnemyAI : MonoBehaviour, IEnemy
{
    private Transform player;
    public float speed = 2f;
    public float detectionRange = 5f;
    public int damageAmount = 10;
    private Rigidbody2D rb;
    private bool isFacingRight = true; // Tracks current facing direction

    public int health = 10;

    public GameObject[] itemDrops; // Array of item prefabs that can drop
    public float dropChance = 1f; // 50% chance to drop an item
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
                Debug.Log("Enemy found the player!");
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
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
                Debug.Log("Enemy attacked the player!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; // Subtract damage from health
        Debug.Log($"{gameObject.name} took {damage} damage! Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        DropItem();
        Destroy(gameObject);
    }
    private void DropItem()
    {
        if (itemDrops.Length > 0 && Random.value < dropChance) // Random chance check
        {
            int randomIndex = Random.Range(0, itemDrops.Length);
            GameObject droppedItem = Instantiate(itemDrops[randomIndex], transform.position, Quaternion.identity);

            // Add Rigidbody2D if missing
            Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = droppedItem.AddComponent<Rigidbody2D>();
            }

            // Make sure it's "Dynamic" so it interacts with physics
            rb.bodyType = RigidbodyType2D.Dynamic;

            // Ensure there's a Collider2D
            if (droppedItem.GetComponent<Collider2D>() == null)
            {
                droppedItem.AddComponent<BoxCollider2D>(); // Or CircleCollider2D depending on the shape
            }

            // Adjust Rigidbody properties for a natural drop
            rb.gravityScale = 2f;  // Falls at a good speed
            rb.linearDamping = 2f;          // Slows down excessive movement
            rb.freezeRotation = true; // Prevents weird spinning

            // Apply a gentle pop-up effect
            rb.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(2f, 3f));
        }
    }

}
