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

    public GameObject[] itemDrops;
    public float dropChance = 1f;

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
    }

    void Update()
    { }

    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (!playerDetected && other.CompareTag("Player"))
        {
            playerDetected = true;

            // Wait for a short period before starting the dash
            yield return new WaitForSeconds(waitTime);

            StartCoroutine(DashToTarget());
        }
    }

    private IEnumerator DashToTarget()
    {
        Vector2 targetPosition = targetSpot.position;

        // Trigger the dash animation
        animator.SetTrigger("Dash");

        // Wait for the duration of the animation (assuming you know the animation's length)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Start moving after the animation duration
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;  // Ensure the boss reaches the target position

        // Flip the boss after reaching the target
        FlipAfterDash();

        // Switch the target spot
        SwitchTargetSpot();

        // Wait before the next dash
        yield return new WaitForSeconds(waitTime);

        // Start the next dash cycle
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

    // Implement the TakeDamage method as required by the IEnemy interface
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
        DropItem();
        Destroy(gameObject);
    }

    private void DropItem()
    {
        if (itemDrops.Length > 0 && Random.value < dropChance)
        {
            int randomIndex = Random.Range(0, itemDrops.Length);
            Instantiate(itemDrops[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
