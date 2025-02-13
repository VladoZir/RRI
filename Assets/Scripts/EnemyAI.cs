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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
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
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                //Debug.Log("Enemy attacked the player!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; 
       // Debug.Log($"{gameObject.name} took {damage} damage! Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Debug.Log($"{gameObject.name} has died.");
        DropItem();
        Destroy(gameObject);
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

            float upwardForce = 5f;  
            float sidewaysForce = Random.Range(-2f, 2f); 
            rb.linearVelocity = new Vector2(sidewaysForce, upwardForce);

            droppedItem.AddComponent<ItemCollisionHandler>();

            // Destroy the item after 10 seconds
            Destroy(droppedItem, 10f);
        }
    }
}