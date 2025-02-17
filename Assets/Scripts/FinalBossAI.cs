 using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class FinalBossAI : MonoBehaviour, IEnemy
{
    public AIPath aiPath;
    
    public int maxHealth = 300;
    public int currentHealth;
    public List<Sprite> healthSprites = new List<Sprite>();
    public GameObject healthContainer;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float hitColorDuration = 0.25f;
    public int damage = 10;

    private Animator animator;

    public CircleCollider2D bossCollider;

    public GameObject portalPrefab;
    public Transform spawnPoint;

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    private int nextSpawnThreshold;

    public GameObject projectilePrefab;
    public Transform shootPoint; 
    public float shootCooldown = 2f; 
    private float nextShootTime = 0f;

    public float projectileSpeed = 20f;
    private Transform playerTransform; 
    private AIDestinationSetter aiDestinationSetter;
    private bool isActivated = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        bossCollider = GetComponent<CircleCollider2D>();

        nextSpawnThreshold = Mathf.FloorToInt(maxHealth * 0.75f); // First spawn at 75% health

        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player = GameManager.Instance?.currentPlayer;
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
           FindPlayer();
        }

        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (aiPath.desiredVelocity.x <= -0.01)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (isActivated && Time.time >= nextShootTime)
        {
            ShootAtPlayer();
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab != null && shootPoint != null && playerTransform != null)
        {
            animator.SetBool("IsShooting", true);

            // Calculate direction to the player using Transform
            Vector2 direction = (playerTransform.position - shootPoint.position).normalized;

            // Instantiate projectile at shootPoint
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            // Add Rigidbody2D component if it doesn't exist
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = projectile.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // Disable gravity for the projectile
            }

            // Set the velocity of the projectile
            rb.linearVelocity = direction * projectileSpeed;

            // Optionally, rotate the projectile to face the direction it's moving
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle-180f, Vector3.forward);

            nextShootTime = Time.time + shootCooldown;
            
            StartCoroutine(ResetShootTrigger());
        }
    }

    private IEnumerator ResetShootTrigger()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("FinalBossShoot") == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        // Reset the "Shoot" trigger
        animator.SetBool("IsShooting", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            Collider2D playerCollider = collision.gameObject.GetComponent<Collider2D>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
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
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(playerCollider, enemyCollider, false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        changeHealthSprite(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        
        else if (currentHealth <= nextSpawnThreshold) 
        {
            SpawnEnemies();
            nextSpawnThreshold -= Mathf.FloorToInt(maxHealth * 0.25f); // Move to next 25% threshold
        }
        



        StartCoroutine(ChangeColorOnHit());
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) return;

        Transform playerTransform = GameManager.Instance?.currentPlayer?.transform; 

        int spawnCount = Mathf.Min(4, spawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnLocation = spawnPoints[i];

            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);

            // Immediately set the AI target if the player is already detected
            AIDestinationSetter aiDestination = spawnedEnemy.GetComponent<AIDestinationSetter>();
            if (aiDestination != null && playerTransform != null)
            {
                aiDestination.target = playerTransform;
            }
        }
    }

    public void OnHealthBarActivated()
    {
        healthContainer = GameObject.Find("FinalBossHealthBar");
        isActivated = true;

        if (healthContainer != null)
        {
            //Debug.Log("Health bar found!");
        }
        else
        {
            //Debug.LogError("FinalBossHealthBar not found!");
        }
    }


    public void changeHealthSprite(int curHealth)
    {
        // Ako je zdravlje 0, uvijek prikazuj sprite s indeksom 0
        if (curHealth == 0)
        {
            healthContainer.GetComponent<Image>().sprite = healthSprites[0];
            Debug.Log("Health: " + curHealth + " | Sprite Index: 0");
            return;
        }

        // Izračunaj postotak od maksimalnog zdravlja
        int healthPercentage = (curHealth * 100) / maxHealth;

        // Na temelju postotka, odaberi odgovarajući sprite
        int spriteIndex = Mathf.FloorToInt(healthPercentage / 10); // Za svakih 10%

        // Osiguraj da je spriteIndex u granicama (0 do broj spriteova - 1)
        spriteIndex = Mathf.Clamp(spriteIndex, 1, healthSprites.Count - 1); // Minimum je 1, tako da sprite 0 nije prikazan dok zdravlje nije 0

        // Postavi odgovarajući sprite za zdravlje
        healthContainer.GetComponent<Image>().sprite = healthSprites[spriteIndex];

        //Debug.Log("Health: " + curHealth + " | Sprite Index: " + spriteIndex);
    }



    private IEnumerator ChangeColorOnHit()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitColorDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        animator.SetBool("IsDead", true);

        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");

        StopAllCoroutines();

        AIPath AIPath = GetComponent<AIPath>();
        if (AIPath != null)
        {
            AIPath.canMove = false;  // Stops AI movement
            AIPath.enabled = false;  // Disables AIPath component
            //AIPath.gravity = new Vector3(0, -9.81f, 0);
        }

        AIDestinationSetter AIDestination = GetComponent<AIDestinationSetter>();
        if (AIDestination != null)
        {
            AIDestination.enabled = false;  // Disables the AI target tracking
        }

        Seeker seeker = GetComponent<Seeker>();
        if (seeker != null)
        {
            seeker.enabled = false;  // Disables path recalculations
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f; // Set gravity so he falls
            rb.linearVelocity = Vector2.down * 2f; // Give him a little downward push
        }

        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("FinalBossDeath") == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }


        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;

        Instantiate(portalPrefab, spawnPoint.position, Quaternion.identity);

        Destroy(gameObject);
    }


    private void OnEnable()
    {
        AITrigger.OnPlayerTriggered += SetAITarget; // Subscribe to the event
    }

    private void OnDisable()
    {
        AITrigger.OnPlayerTriggered -= SetAITarget; // Unsubscribe to avoid memory leaks
    }

    private void SetAITarget(Transform playerTransform)
    {
        AIDestinationSetter aiDestination = GetComponent<AIDestinationSetter>();
        if (aiDestination != null)
        {
            aiDestination.target = playerTransform;
            //Debug.Log("Final Boss AI now follows the player.");
        }
    }

}
