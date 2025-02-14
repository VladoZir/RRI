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

    //public PolygonCollider2D idleCollider;
    //public CircleCollider2D atackCollider;

    public GameObject healthBarHolder;

    public List<Sprite> healthSprites = new List<Sprite>();

    public GameObject portalPrefab;
    public Transform spawnPoint;

    private bool isMoving = false;
    public float moveSpeed = 20f;
    public List<Transform> targetSpots;
    private int currentTargetIndex = 0;

    //public GameObject[] enemyPrefabs;
    //public Transform[] spawnPoints;
    //private int nextSpawnThreshold;

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
        //nextSpawnThreshold = Mathf.FloorToInt(maxHealth * 0.75f); // First spawn at 75% health

    }
    private IEnumerator MoveToTargetSpots()
    {
        isMoving = true;

        // Loop through target spots in the original order
        for (int i = 0; i < targetSpots.Count; i++)
        {
            Transform targetSpot = targetSpots[i];
            yield return StartCoroutine(MoveToTarget(targetSpot));
        }

        // After reaching all targets, move back to the starting position by reversing the target spots list
        for (int i = targetSpots.Count - 1; i >= 0; i--)
        {
            Transform targetSpot = targetSpots[i];
            yield return StartCoroutine(MoveToTarget(targetSpot));
        }

        // Finally, return to the starting position
        yield return StartCoroutine(MoveToTarget(transform)); // Assuming transform is the starting position

        isMoving = false;
    }

    // Move towards a specific target position without stopping at it
    private IEnumerator MoveToTarget(Transform target)
    {
        float moveThreshold = 0.2f; // A small threshold to stop "hitting" the target spot
        while (Vector2.Distance(transform.position, target.position) > moveThreshold)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);

            // You can add more logic here like setting an animation for movement if needed

            yield return null;
        }
    }


    // Detect player in range and start the movement
    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (!playerDetected && other.CompareTag("Player"))
        {
            Debug.Log("trigger");
            playerDetected = true;

            yield return new WaitForSeconds(waitTime);

            StartCoroutine(MoveToTargetSpots());
        }
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

            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackForce = new Vector2(0, 10f);
                playerRb.AddForce(knockbackForce, ForceMode2D.Impulse);
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
        }/*
        else if (curHealth <= nextSpawnThreshold) // Check if boss health crossed the next threshold
        {
            SpawnEnemies();
            nextSpawnThreshold -= Mathf.FloorToInt(maxHealth * 0.25f); // Move to next 25% threshold
        }
        */

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

        StopAllCoroutines();

        this.enabled = false;

        //StartCoroutine(WaitForDeathAnimation());
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

    private IEnumerator WaitForDeathAnimation()
    {
        
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("DinoDeathNew") == false)
        {
            yield return null;
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

    /*
   private void SpawnEnemies()
   {
       if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;
       int spawnCount = Mathf.Min(4, spawnPoints.Length); // Ensure we don't spawn more than we have spawn points

       for (int i = 0; i < spawnCount; i++) // Loop over each spawn point
       {
           GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]; // Randomly pick an enemy from the array
           Transform spawnLocation = spawnPoints[i]; // Use a different spawn point for each enemy

           Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.identity); // Spawn the enemy at the spawn point
       }

   }
   */
}
