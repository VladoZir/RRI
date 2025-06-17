using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class DinoBossAI : MonoBehaviour, IEnemy
{

    public float dashSpeed = 8f;
    public float waitTime = 1f;
    public float detectionRange = 15f;
    private bool playerDetected = false;
    public Transform leftSpot;
    public Transform rightSpot;
    private Transform targetSpot;

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
    public PolygonCollider2D dashCollider;

    public GameObject healthBarHolder;

    public List<Sprite> healthSprites = new List<Sprite>();

    public GameObject portalPrefab;
    public Transform spawnPoint;

    public GameObject[] enemyPrefabs; 
    public Transform[] spawnPoints; 
    private int nextSpawnThreshold;

    void Start()
    {
        curHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        targetSpot = leftSpot;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject)
        {
            player = playerObject.transform;
        }

        healthBarHolder = GameObject.Find("BossHealthBarHolder");
        nextSpawnThreshold = Mathf.FloorToInt(maxHealth * 0.75f); 

        EnableIdleCollider();
    }



    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (!playerDetected && other.CompareTag("Player"))
        {
            playerDetected = true;

            yield return new WaitForSeconds(waitTime);

            StartCoroutine(DashToTarget());
        }
    }

    private IEnumerator DashToTarget()
    {
        Vector2 targetPosition = targetSpot.position;

        EnableDashCollider();

        animator.SetTrigger("Dash");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;

        FlipAfterDash();
        SwitchTargetSpot();

        EnableIdleCollider();

        yield return new WaitForSeconds(waitTime);

        StartCoroutine(DashToTarget());
    }

    private void FlipAfterDash()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        Vector3 healthBarScale = transform.GetChild(0).localScale;
        transform.GetChild(0).localScale = new Vector3(Mathf.Abs(healthBarScale.x) * Mathf.Sign(transform.localScale.x), healthBarScale.y, healthBarScale.z);
    }

    private void SwitchTargetSpot()
    {
        targetSpot = (targetSpot == rightSpot) ? leftSpot : rightSpot;
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


    private void EnableIdleCollider()
    {
        idleCollider.enabled = true;
        dashCollider.enabled = false;
    }

    private void EnableDashCollider()
    {
        idleCollider.enabled = false;
        dashCollider.enabled = true;
    }

    public void TakeDamage(int damage)
    {
        curHealth -= damage;

        changeHealthSprite(curHealth);

        if (curHealth <= 0)
        {
            Die();
        }
        else if (curHealth <= nextSpawnThreshold) 
        {
            SpawnEnemies();
            nextSpawnThreshold -= Mathf.FloorToInt(maxHealth * 0.25f);
        }

        StartCoroutine(ChangeColorOnHit());
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;
        int spawnCount = Mathf.Min(4, spawnPoints.Length); 

        for (int i = 0; i < spawnCount; i++) 
        {
            GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]; 
            Transform spawnLocation = spawnPoints[i]; 

            Instantiate(enemyToSpawn, spawnLocation.position, Quaternion.identity); 
        }

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

        
        idleCollider.enabled = false;
        dashCollider.enabled = false;

        StopAllCoroutines();

        this.enabled = false;

        StartCoroutine(WaitForDeathAnimation());
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

        float upwardForce = 3f;
        float sidewaysForce = Random.Range(-1f, 1f);
        rb.linearVelocity = new Vector2(sidewaysForce, upwardForce);

        droppedItem.AddComponent<ItemCollisionHandler>();

        Instantiate(portalPrefab, spawnPoint.position, Quaternion.identity);

        Destroy(gameObject); 
    }
}