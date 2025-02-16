 using UnityEngine;
using Pathfinding;
using System.Collections;

public class FinalBossAI : MonoBehaviour, IEnemy
{
    public AIPath aiPath;

    public int maxHealth = 100;
    public int curHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float hitColorDuration = 0.25f;
    public int damage = 10;

    private Animator animator;

    public CircleCollider2D bossCollider;

    public GameObject portalPrefab;
    public Transform spawnPoint;

    //public GameObject enemyPrefab;
    //public Transform[] spawnPoints;
    //private int nextSpawnThreshold;

    void Start()
    {
        curHealth = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        bossCollider = GetComponent<CircleCollider2D>();

        //nextSpawnThreshold = Mathf.FloorToInt(maxHealth * 0.75f); // First spawn at 75% health
    }

    void Update()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f,1f,1f);
        }
        else if(aiPath.desiredVelocity.x <= -0.01)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
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

        //changeHealthSprite(curHealth);

        if (curHealth <= 0)
        {
            Die();
        }
        /*
        else if (curHealth <= nextSpawnThreshold) // Check if boss health crossed the next threshold
        {
            SpawnEnemies();
            nextSpawnThreshold -= Mathf.FloorToInt(maxHealth * 0.25f); // Move to next 25% threshold
        }
        */



        StartCoroutine(ChangeColorOnHit());
    }

    /*
    public void changeHealthSprite(int curHealth)
    {
        int index = Mathf.Clamp(curHealth / 10, 0, healthSprites.Count - 1);
        healthBarHolder.GetComponent<Image>().sprite = healthSprites[index];

    }
    */


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


}
