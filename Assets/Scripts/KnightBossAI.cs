using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnightBossAI : MonoBehaviour, IEnemy
{
    public float waitTime = 1f;
    public float detectionRange = 15f;
    private bool playerDetected = false;

    private Animator animator;
    private Transform player;

    public int maxHealth = 100;
    public int curHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float hitColorDuration = 0.25f;

    public GameObject medkit;
    public int damage = 20;

    public PolygonCollider2D idleCollider;
    public CircleCollider2D attackCollider;
    public BoxCollider2D boxCollider;

    public GameObject healthBarHolder;

    public List<Sprite> healthSprites = new List<Sprite>();

    public GameObject portalPrefab;
    public Transform spawnPoint;

    public float moveSpeed = 20f;
    public List<Transform> targetSpots;

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    private int nextSpawnThreshold;

    void Start()
    {
        curHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject)
        {
            player = playerObject.transform;
        }

        healthBarHolder = GameObject.Find("BossHealthBarHolder");
        nextSpawnThreshold = Mathf.FloorToInt(maxHealth * 0.75f); // First spawn at 75% health


        EnableIdleCollider();
    }
    void Update()
    {
        if (playerDetected)
        {
            boxCollider.enabled = false;
        }
    }

    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (!playerDetected && other.CompareTag("Player"))
        {
            playerDetected = true;

            yield return new WaitForSeconds(waitTime);

            StartCoroutine(MoveToTargetSpots());
        }
    }
    private IEnumerator MoveToTargetSpots()
    {
        EnableAttackCollider();
        while (playerDetected) 
        {
            animator.SetTrigger("StartMove");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool("IsMoving", true);

            // Move through target spots in order
            for (int i = 0; i < targetSpots.Count; i++)
            {
                Transform targetSpot = targetSpots[i];
                yield return StartCoroutine(MoveToTarget(targetSpot));
            }

            // Move back through the target spots in reverse
            for (int i = targetSpots.Count - 1; i >= 0; i--)
            {
                Transform targetSpot = targetSpots[i];
                yield return StartCoroutine(MoveToTarget(targetSpot));
            }

            // Return to the starting position
            yield return StartCoroutine(MoveToTarget(transform));

            animator.SetBool("IsMoving", false);

            yield return new WaitForSeconds(3f);
        }
        EnableIdleCollider();
    }
    // Move towards a specific target position without stopping at it
    private IEnumerator MoveToTarget(Transform target)
    {
        float moveThreshold = 0.5f; // A small threshold to stop "hitting" the target spot
        while (Vector2.Distance(transform.position, target.position) > moveThreshold)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // You can add more logic here like setting an animation for movement if needed

            yield return null;
        }
    }

    private void EnableIdleCollider()
    {
        idleCollider.enabled = true;
        attackCollider.enabled = false;
    }

    private void EnableAttackCollider()
    {
        idleCollider.enabled = false;
        attackCollider.enabled = true;
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
        curHealth -= damage;

        changeHealthSprite(curHealth);

        if (curHealth <= 0)
        {
            Die();
        }
        
        else if (curHealth <= nextSpawnThreshold) // Check if boss health crossed the next threshold
        {
            SpawnEnemies();
            nextSpawnThreshold -= Mathf.FloorToInt(maxHealth * 0.25f); // Move to next 25% threshold
        }
        
        

        StartCoroutine(ChangeColorOnHit());
    }

    public void changeHealthSprite(int curHealth)
    {
        int index = Mathf.Clamp(curHealth / 10, 0, healthSprites.Count - 1);
        healthBarHolder.GetComponent<Image>().sprite = healthSprites[index];

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

        StopAllCoroutines();

        idleCollider.enabled = false;
        attackCollider.enabled = false;

        this.enabled = false;

        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("KnightBossDeath") == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        

        Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
        GameObject droppedItem = Instantiate(medkit, spawnPosition, Quaternion.identity);

        Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = droppedItem.AddComponent<Rigidbody2D>();
        }

        float upwardForce = 5f;
        float sidewaysForce = Random.Range(-2f, 2f);
        rb.linearVelocity = new Vector2(sidewaysForce, upwardForce);

        droppedItem.AddComponent<ItemCollisionHandler>();

        Instantiate(portalPrefab, spawnPoint.position, Quaternion.identity);

        Destroy(gameObject);
    }

    
   private void SpawnEnemies()
   {
       //if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;
       int spawnCount = Mathf.Min(4, spawnPoints.Length); // Ensure we don't spawn more than we have spawn points

       for (int i = 0; i < spawnCount; i++) // Loop over each spawn point
       {
           //GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]; // Randomly pick an enemy from the array
           Transform spawnLocation = spawnPoints[i]; // Use a different spawn point for each enemy

           Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity); // Spawn the enemy at the spawn point
       }

   }
   
}
