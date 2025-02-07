using UnityEngine;

public class HealthItems : MonoBehaviour
{
    public int potionHealAmount = 20;
    public int medkitHealAmount = 50;
    public int shieldPotionAmount = 50;

    private Collider2D itemCollider;
    private Rigidbody2D rb;
    private bool hasLanded = false;

    void Start()
    {
        itemCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 1f;  // Enable falling
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            rb.bodyType = RigidbodyType2D.Kinematic; // Stop physics movement
            rb.linearVelocity = Vector2.zero;
            itemCollider.isTrigger = true; // Enable pass-through for player
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (CompareTag("ShieldPotion"))
                {
                    if (playerHealth.currentShield < playerHealth.maxShield)
                    {
                        playerHealth.RestoreShield(shieldPotionAmount);
                        Destroy(gameObject);
                        Debug.Log("Shield Potion Used! Current Shield: " + playerHealth.currentShield);
                    }
                    else
                    {
                        Debug.Log("Shield is already full! Item not used.");
                    }
                }
                else if (playerHealth.currentHealth < playerHealth.maxHealth)
                {
                    if (CompareTag("HealthPotion"))
                    {
                        playerHealth.Heal(potionHealAmount);
                        Destroy(gameObject);
                        Debug.Log("Health Potion Used! Current Health: " + playerHealth.currentHealth);
                    }
                    else if (CompareTag("Medkit"))
                    {
                        playerHealth.Heal(medkitHealAmount);
                        Destroy(gameObject);
                        Debug.Log("Medkit Used! Current Health: " + playerHealth.currentHealth);
                    }
                }
                else
                {
                    Debug.Log("Health is already full! Item not used.");
                }
            }
        }
    }
}
