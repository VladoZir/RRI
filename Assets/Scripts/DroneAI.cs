using Pathfinding;
using System.Collections;
using UnityEngine;

public class DroneAI : MonoBehaviour, IEnemy
{
    public AIPath aiPath;

    private Transform player;
    public float speed = 2f;
    public float detectionRange = 5f;
    public int damageAmount = 10;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    public int health = 10;

    public GameObject[] itemDrops;
    public float dropChance = 0.25f;

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
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (aiPath.desiredVelocity.x <= -0.01)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
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

            float upwardForce = 5f;
            float sidewaysForce = Random.Range(-2f, 2f);
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
