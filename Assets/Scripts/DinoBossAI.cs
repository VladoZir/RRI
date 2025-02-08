using UnityEngine;
using System.Collections;

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

            // Ensure DinoBoss keeps moving after hitting the player
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
            Invoke(nameof(ResetCollision), 0.2f); // Re-enable collision after short delay

        }
    }

    private void ResetCollision()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), false);
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
        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(ChangeColorOnHit());
    }

    private IEnumerator ChangeColorOnHit()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitColorDuration);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        Instantiate(medkit, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
