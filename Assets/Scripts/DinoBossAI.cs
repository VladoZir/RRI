using UnityEngine;
using System.Collections;

public class DinoBossAI : MonoBehaviour, IEnemy
{
    public float speed = 3f;            
    public float detectionRange = 25f;
    public float dashSpeed = 8f;      
    public float dashCooldown = 3f;   
    public float dashDuration = 0.5f;  
    private bool isDashing = false;     
    private bool isCoolingDown = false;
    private float cooldownTimer = 0f;   

    public Transform player;            
    private Animator animator;         
    private Rigidbody2D rb;             
    private bool playerFound = false;

    public int health = 100;

    private SpriteRenderer spriteRenderer; 
    private Color originalColor;
    public float hitColorDuration = 0.5f;


    void Start()
    {
        animator = GetComponent<Animator>();  
        rb = GetComponent<Rigidbody2D>();    

        spriteRenderer = GetComponent<SpriteRenderer>();  
        originalColor = spriteRenderer.color;

    }

    void Update()
    {
        if (!playerFound)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                // Check if the player object has a child named "Bow"
                Transform bowTransform = playerObject.transform.Find("Bow");
                if (bowTransform != null)
                {
                    player = playerObject.transform;  // Set the player reference
                    playerFound = true; // Mark player as found
                }/*
                else
                {
                    Debug.LogWarning("Player object does not have a 'Bow' child!");
                }*/
            }
            else
            {
                Debug.LogWarning("No object with Player tag found!");
            }
        }


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

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"DinoBoss took {damage} damage! Health: {health}");

        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(ChangeColorOnHit());
    }

    private IEnumerator ChangeColorOnHit()
    {
        // Change the color to red
        spriteRenderer.color = Color.red;

        // Wait for the specified duration
        yield return new WaitForSeconds(hitColorDuration);

        // Reset the color back to the original
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("DinoBoss has died.");
        Destroy(gameObject);
    }

}
