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

    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootCooldown = 2f;
    private float nextShootTime = 0f;
    public float projectileSpeed = 20f;
    private Transform playerTransform;
    private AIDestinationSetter aiDestinationSetter;

    void Start()
    {
        animator = GetComponent<Animator>();
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

        if (Time.time >= nextShootTime)
        {
            ShootAtPlayer();
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab != null && shootPoint != null && playerTransform != null)
        {

            animator.SetBool("IsShooting", true);

            Vector2 direction = (playerTransform.position - shootPoint.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = projectile.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; 
            }

            rb.linearVelocity = direction * projectileSpeed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(angle - 180f, Vector3.forward);

            nextShootTime = Time.time + Random.Range(0f, 2f);

            StartCoroutine(ResetShootTrigger());
        }
    }

    private IEnumerator ResetShootTrigger()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("DroneShoot") == false)
        {
            yield return new WaitForSeconds(0.1f);
        }

        animator.SetBool("IsShooting", false);
    }


    public void TakeDamage(int damage)
    {
        health -= damage;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; 
        }

        if (health <= 0)
        {
            Die(); 
        }
        else
        {
            Invoke(nameof(ResetColor), 0.2f); 
        }
    }


    private void Die()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; 
        }

        OnDeath?.Invoke(gameObject);
        DropItem();

        Destroy(gameObject, 0.1f); 
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

            Destroy(droppedItem, 10f);
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; 

        }
    }

}
