using UnityEngine;

public class HealthItems : MonoBehaviour
{
    public int potionHealAmount = 20;
    public int medkitHealAmount = 50;
    public int shieldPotionAmount = 50;

    

    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
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
