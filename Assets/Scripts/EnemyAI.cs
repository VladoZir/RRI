using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IEnemy
{
    private Transform player;
    public float speed = 2f;
    public float detectionRange = 5f;
    public int damageAmount = 10;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    public int health = 10;

    public GameObject[] itemDrops;
    public float dropChance = 0.1f;

    private Animator animator;

    public delegate void DeathEvent(GameObject enemy);
    public event DeathEvent OnDeath;

    private SpriteRenderer spriteRenderer;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }


    void Update()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
                //Debug.Log("Enemy found the player!");
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;

            rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);

            animator.SetBool("isWalking", true); // Start walking animation

            if ((direction.x > 0 && !isFacingRight) || (direction.x < 0 && isFacingRight))
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false); // Stop animation when idle
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
            Collider2D playerCollider = collision.gameObject.GetComponent<Collider2D>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                //Debug.Log("Enemy attacked the player!");
            }
            if (playerCollider != null)
            {
                StartCoroutine(DisablePlayerColliderTemporarily(playerCollider, collision.collider));
            }
        }
    }

    private IEnumerator DisablePlayerColliderTemporarily(Collider2D playerCollider, Collider2D enemyCollider)
    {
        Physics2D.IgnoreCollision(playerCollider, enemyCollider, true);
        yield return new WaitForSeconds(1.5f);
        Physics2D.IgnoreCollision(playerCollider, enemyCollider, false);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // Turn red when hit
        }

        if (health <= 0)
        {
            Die(); // Kill the enemy immediately
        }
        else
        {
            Invoke(nameof(ResetColor), 0.2f); // Only reset color if still alive
        }
    }


    private void Die()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // Turn red before death
        }

        OnDeath?.Invoke(gameObject);
        DropItem();

        Destroy(gameObject, 0.1f); // Small delay before destruction
    }
    private void DropItem()
    {
        if (itemDrops.Length > 0 && Random.value < dropChance)
        {
            int randomIndex = Random.Range(0, itemDrops.Length);
            Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
            GameObject droppedItem = Instantiate(itemDrops[randomIndex], spawnPosition, Quaternion.identity);

            Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = droppedItem.AddComponent<Rigidbody2D>();
            }
            rb.freezeRotation = true;

            float upwardForce = 3f;
            float sidewaysForce = Random.Range(-1f, 1f);
            rb.linearVelocity = new Vector2(sidewaysForce, upwardForce);

            droppedItem.AddComponent<ItemCollisionHandler>();

            // Destroy the item after 10 seconds
            Destroy(droppedItem, 10f);
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; // Default color
        }
    }
}