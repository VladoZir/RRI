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

    public int health = 100;
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

    void Start()
    {
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

        // Wait for the duration of the animation (assuming you know the animation's length)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Start moving after the animation duration
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
        // Simply flip the boss by multiplying the X value of the local scale by -1
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);


        Vector3 healthBarScale = transform.GetChild(0).localScale;
        transform.GetChild(0).localScale = new Vector3(Mathf.Abs(healthBarScale.x) * Mathf.Sign(transform.localScale.x), healthBarScale.y, healthBarScale.z);
    }

    private void SwitchTargetSpot()
    {
        // Switch between the left and right spots
        targetSpot = (targetSpot == rightSpot) ? leftSpot : rightSpot;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            Collider2D playerCollider = collision.gameObject.GetComponent<Collider2D>(); // Get player's collider

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackForce = new Vector2(0, 10f); // Adjust force as needed
                playerRb.AddForce(knockbackForce, ForceMode2D.Impulse);
            }

            // Disable player's collider temporarily to avoid continuous pushing
            if (playerCollider != null)
            {
                StartCoroutine(DisablePlayerColliderTemporarily(playerCollider));
            }
        }
    }

    private IEnumerator DisablePlayerColliderTemporarily(Collider2D playerCollider)
    {
        playerCollider.enabled = false;
        yield return new WaitForSeconds(0.5f); // Adjust delay as needed
        playerCollider.enabled = true;
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
        health -= damage;
        //healthBar.value = health;  // Update the health bar

        changeHealthSprite(health);

        if (health <= 0)
        {
            Die();
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
        // Ensure the death animation is triggered
        animator.SetBool("IsDead", true);

        // Disable colliders so the boss no longer interacts with the player
        idleCollider.enabled = false;
        dashCollider.enabled = false;

        // Stop all boss actions
        StopAllCoroutines();

        // Disable movement and AI logic
        this.enabled = false;

        // Wait for the death animation to finish before doing any other logic
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("DinoDeathNew") == false)
        {
            yield return null;
        }

        // Wait for the animation to finish
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
}